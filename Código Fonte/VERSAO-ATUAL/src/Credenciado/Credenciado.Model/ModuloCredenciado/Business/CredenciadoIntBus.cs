using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloEmail.Business;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloHabilitarEmissaoCFOCFOC.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloOrgaoParceiroConveniado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business
{
    public class CredenciadoIntBus
    {
        #region Propriedades
        OrgaoParceiroConveniadoBus _busOrgaoParceiro;
        CredenciadoIntDa _daCredenciado;

        HabilitarEmissaoCFOCFOCDa _daHabilitar;
        CredenciadoBus _busCredenciado;
        PessoaBus _busPessoa;
        CredenciadoIntValidar _validar;
        EmailBus _emailBus;
        ListaBus _busLista;
        GerenciadorConfiguracao<ConfiguracaoCredenciado> _config = new GerenciadorConfiguracao<ConfiguracaoCredenciado>(new ConfiguracaoCredenciado());
        GerenciadorConfiguracao<ConfiguracaoUsuario> _configUsuario = new GerenciadorConfiguracao<ConfiguracaoUsuario>(new ConfiguracaoUsuario());
        GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

        public String CredenciadoLinkAcesso { get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyCredenciadoLinkAcesso); } }

        #endregion

        public CredenciadoIntBus()
        {
            _busCredenciado = new CredenciadoBus();
            _validar = new CredenciadoIntValidar();
            _daCredenciado = new CredenciadoIntDa();
            _daHabilitar = new HabilitarEmissaoCFOCFOCDa();
            _busPessoa = new PessoaBus();
            _emailBus = new EmailBus();
            _busLista = new ListaBus();
            _busOrgaoParceiro = new OrgaoParceiroConveniadoBus();
        }

        public Resultados<Cred.ListarFiltro> Filtrar(Cred.ListarFiltro filtrosListar, Paginacao paginacao)
        {
            try
            {
                return _busCredenciado.Filtrar(filtrosListar, paginacao);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return null;
        }

        public CredenciadoPessoa Obter(int id)
        {
            return _busCredenciado.Obter(id);
        }

        public CredenciadoPessoa Obter(String CpfCnpj)
        {
            if (ValidacoesGenericasBus.Cpf(CpfCnpj))
            {
                var retorno = _busCredenciado.Obter(CpfCnpj);
                if (retorno != null && retorno.Id > 0)
                {
                    if (_daHabilitar.ValidarResponsavelHabilitado(retorno.Id))
                    {
                        Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.ResponsavelHabilitado);
                    }

                    if (retorno.Pessoa.Fisica.Profissao.Id == 0 || String.IsNullOrWhiteSpace(retorno.Pessoa.Fisica.Profissao.Registro))
                    {
                        Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.SemProfissaoRegistro);
                    }

                    return retorno;
                }
                else
                {
                    Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.CpfNaoCadastrado);
                }
            }
            else
            {
                Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.CpfInvalido);
            }

            return null;
        }

        public CredenciadoPessoa ObterPorCPF(String cpf)
        {
            try
            {
                return _busCredenciado.Obter(cpf);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return null;
        }
        public Pessoa ObterPessoaCredenciado(int pessoaId)
        {
            return _busCredenciado.ObterPessoaCredenciado(pessoaId);
        }

        public string GerarChaveAcesso(string email, string nome)
        {
            byte[] byteHash = null;

            try
            {
                string strTexto = email.ToLower() + "$" + DateTime.Now.Ticks.ToString() + "$" + nome;
                UTF8Encoding encoder = new UTF8Encoding();
                SHA512 sha512 = SHA512.Create();
                byteHash = sha512.ComputeHash(encoder.GetBytes(strTexto));
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return string.Join("", byteHash.Select(bin => bin.ToString("X2")).ToArray());
        }

        public bool EnviarEmail(CredenciadoPessoa credenciado, string email, bool cadastro = true, BancoDeDados banco = null)
        {
            return _busCredenciado.EnviarEmail(credenciado, email, cadastro, banco);
        }

        public void AlterarSituacao(CredenciadoPessoa credenciado, BancoDeDados banco = null)
        {
            _busCredenciado.AlterarSituacao(credenciado, banco);
        }

        public bool AlterarSituacao(int id, string nome, int novaSituacao, string motivo)
        {
            return _busCredenciado.AlterarSituacao(id, nome, novaSituacao, motivo);
        }

        public void RegerarChave(int id)
        {
            CredenciadoPessoa credenciado = _busCredenciado.Obter(id, true);
            OrgaoParceiroConveniado orgao = _busOrgaoParceiro.Obter(credenciado.OrgaoParceiroId);

            if (orgao.SituacaoId == (int)eOrgaoParceiroConveniadoSituacao.Bloqueado)
            {
                Mensagem mensagem = Mensagem.OrgaoParceiroConveniado.OrgaoBloqueado;
                mensagem.Texto = mensagem.Texto.Replace("#nome#", orgao.Nome);
                Validacao.Add(mensagem);
            }

            if (!_validar.RegerarChave(credenciado))
            {
                return;
            }

            _busCredenciado.RegerarChave(id);
        }
    }
}
