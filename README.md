# 1. Cenário Geral
De forma ampla, aplicações modernas interagem da seguinte forma:

-	Browers comunicam com Web Applications;
-	Web Applications comunicam com Web APIs;
-	Aplicações nativas comunicam com Web APIs;

Tipicamente, cada camada deve implementar autenticação e/ou autorização. Portanto, isolar essas funções de segurança para um serviço externo evita a duplicidade de código entre todos os envolvidos.

# 2. Autenticação, OpenID e OAuth 2.0
A autenticação é necessária para que uma aplicação possa conhecer a identidade do usuário. Os protocolos de autenticação mais comuns são SAML2p, WS-Federation and OpenID Connect, sendo que o SAML2p é o mais popular e mais implementado globalmente.

O mais recente entre os três é o OpenID e é considerado o futuro, pois ele foi desenvolvido do zero, com maior potencial para aplicações modernas e projetado para ser amigável com APIs.

OpenID e o OAuth 2.0 são bem similares, na verdade o OpenID é uma extensão do OAuth 2.0. Portanto, as preocupações fundamentais de segurança estão unidas em um único protocolo: OpenID.

# 3. IdentityServer4
IdentityServer é um framework que implementa os protocolos OpenID e OAuth 2.0, é gratuito e open source e está disponível para ASP.NETCore. 

Algumas das características do IdentityServer4 são: proteger os **Recursos**, gerenciar e autenticar **Clientes**, autenticar **Usuários**, validar **Tokens**.

Alguns conceitos devem ser destacados:
-	**Recursos**a são implementações que você deseja proteger com o IdentityServer; exemplo: uma API.
-	**Clientes** são os sistemas utilizados pelos Usuários, ou, parte de um sistema; exemplo: aplicação nativa, SPAs ou Web Application. Os Clientes devem obrigatoriamente ser registrados no IdentityServer.
-	**Usuários** são as pessoas, ou seja, é o usuário final que utiliza um Cliente para acessar um Recurso.
-	**Identity Token** representa a saída do processo de autenticação. Contém informações sobre como e quando o usuário autenticou.
-	**Access Token** contém informações sobre o Cliente e o Usuário e são encaminhados para as API de Recursos.

# 4. Demo GitHub
## 4.1. Projeto
A Solução deste repositório contém quatro Projetos:
- **ApiOne** representa uma API que exige autenticação;
- **ClientJavaScript** como o nome sugere, representa um Cliente JavaScript;
- **ClienteMVC** como o nome sugere, representa um Cliente ASP.NET MVC Core;
- E por fim, **IdentityServer**, projeto que implementa o IdentityServer4.
- 
## 4.2. Branches
Três branches estão disponíveis:
-	[**InMemory**](https://github.com/hudsoncadan/Authentication/tree/InMemory) contém um banco de dados em memória;
-	[**Postgres**](https://github.com/hudsoncadan/Authentication/tree/Postgres) contém configurações para o banco de dados Postgres, com inicialização dos dados em Program.cs; 
-	[**Certificate**](https://github.com/hudsoncadan/Authentication/tree/Certificate) contém configurações e documentação para a criação de um Certificado.

# 5. Gerar Certificado
Post que demonstra como criar, ler e excluir certificados. Em resumo, seguem passos abaixo.
https://docs.microsoft.com/en-us/archive/blogs/kaevans/using-powershell-with-certificates

## 5.1. Gerar Certificado
$cert = New-SelfSignedCertificate -Subject "CN=NOME_DO_PROJETO_AQUI" -CertStoreLocation cert:\CurrentUser\My -Provider "Microsoft Strong Cryptographic Provider"

## 5.2. Incluir Credenciais
$cred = Get-Credential

## 5.3. Exportar chave privada
Export-PfxCertificate -Cert $cert -Password $cred.Password -FilePath "c:\temp\sample.pfx"

# 6. IdentityServer4.EntityFramework
Para trabalhar com persistência dos dados, use a biblioteca do IdentityServer4 Entity Framework. Veja detalhes no link abaixo e implementação na branch [**Postgres**](https://github.com/hudsoncadan/Authentication/tree/Postgres).

http://docs.identityserver.io/en/latest/quickstarts/5_entityframework.html

# 7. Fonte
http://docs.identityserver.io/en/latest/index.html
