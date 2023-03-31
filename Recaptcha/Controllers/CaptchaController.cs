using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Recaptcha.Models;
using System.Drawing;

namespace Recaptcha.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        #region Injeções

        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _options;

        #endregion

        #region Construtor

        public CaptchaController(IDistributedCache cache)
        {
            _cache = cache;
            _options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
                SlidingExpiration = TimeSpan.FromSeconds(1200),
            };
        }

        #endregion

        #region Endpoints

        [HttpGet("captcha")]
        public async Task<CaptchaResponseModel> GetCaptcha()
        {
            var token = Guid.NewGuid().ToString("N");
            var captcha = GenerateCaptcha();
            var image = GenerateCaptchaImage(captcha);

            await SetAsync(token.ToString(), token);
            await SetAsync(captcha, captcha);

            return new CaptchaResponseModel
            {
                Token = token,
                Image = image
            };
        }

        [HttpPost("captcha/validate")]
        public async Task<bool> ValidateCaptcha(CaptchaValidationRequestModel model)
        {
            var sessionToken = await GetAsync(model.Token);

            if (string.IsNullOrEmpty(sessionToken))
            {
                return false;
            }

            if (model.Token != sessionToken)
            {
                return false;
            }

            var captcha = await GetAsync(model.Value);

            if (captcha is null)
            {
                return false;
            }

            await DeleteAsync(captcha);
            await DeleteAsync(sessionToken);

            return true;
        }

        #endregion

        #region Métodos Privados

        private byte[] GenerateCaptchaImage(string captcha)
        {
            // Cria uma imagem bitmap para o captcha
            using var bitmap = new Bitmap(200, 80);
            // Cria um objeto Graphics para desenhar na imagem
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Define a cor de fundo da imagem
                graphics.Clear(Color.White);

                // Desenha o captcha na imagem
                using (var font = new Font("Arial", 40))
                {
                    using (var brush = new SolidBrush(Color.Black))
                    {
                        graphics.DrawString(captcha, font, brush, 10, 10);
                    }
                }
            }

            // Converte a imagem bitmap para byte[]
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private static string GenerateCaptcha()
        {
            // Define os caracteres permitidos no captcha
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            // Define o tamanho do captcha
            const int captchaLength = 6;

            // Cria um objeto Random para gerar caracteres aleatórios
            var random = new Random();

            // Gera o captcha aleatório
            var captcha = new string(Enumerable.Repeat(chars, captchaLength).Select(s => s[random.Next(s.Length)]).ToArray());
            return captcha;
        }

        #endregion

        #region Redis

        private async Task SetAsync(string key, string value)
        {
            await _cache.SetStringAsync(key, value, _options);
        }

        private async Task<string> GetAsync(string key) 
        {
            return await _cache.GetStringAsync(key);
        }

        private async Task DeleteAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        #endregion
    }
}
