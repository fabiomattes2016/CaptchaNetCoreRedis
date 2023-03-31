# Captcha/NetCore/Redis

Api para geração de validação captacha

Necessita ter o Redis rodando em alguma vps ou docker

Depois de roadando o Redis é só configurar no arquivo Program.cs na linha 10 as opções de prefixo e conexão

No construtor do controller CaptchaController existe a injeção de DistributedCacheEntryOptions() onde são definidos os tempos
em que os dados ficam no server redis caso ele não remova, essas opções podem ser alteradas.

Ele gera um token e a imagem em formato base64, se quiser testar e visualizar a imagem basta abrir https://codebeautify.org/base64-to-image-converter e colar o base64.

Ele irá validar o token e os caracteres gerados.

Foi usado .Net Core 6
