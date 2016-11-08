using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProjetoDigital.Business
{
    public class ProjetoDigitalBus
    {
        #region Propriedade
        GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
        ProjetoDigitalValidar _validar;
        RequerimentoDa _da;
        Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business.PessoaBus _busPessoa;
        EmpreendimentoBus _empBus;
        ProjetoDigitalCredenciadoBus _busProjetoDigitalCredenciado;
        RequerimentoCredenciadoBus _busRequerimentoCredenciado;

        public String UsuarioCredenciado
        {
            get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
        }

        #endregion

        public ProjetoDigitalBus()
        {
            _validar = new ProjetoDigitalValidar();
            _da = new RequerimentoDa();
            _busPessoa = new Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business.PessoaBus();
            _empBus = new EmpreendimentoBus();
            _busProjetoDigitalCredenciado = new ProjetoDigitalCredenciadoBus();
            _busRequerimentoCredenciado = new RequerimentoCredenciadoBus();
            _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
        }

        #region Ações DML

        public Requerimento Importar(Requerimento requerimento)
        {
            try
            {
                RequerimentoCredenciadoBus busRequerimentoCredenciado = new RequerimentoCredenciadoBus();
                var credenciado = busRequerimentoCredenciado.Obter(requerimento.Id, true);
                credenciado.SetorId = requerimento.SetorId;

                credenciado.Empreendimento.SelecaoTipo = (int)eExecutorTipo.Credenciado;
                credenciado.Pessoas.ForEach(p =>
                {
                    p.SelecaoTipo = (int)eExecutorTipo.Credenciado;
                });

                List<Pessoa> pessoasRelacionadas = ObterPessoasRelacionadas(credenciado);

                if (_validar.Importar(credenciado, pessoasRelacionadas))
                {
                    credenciado.IsCredenciado = true;
                    GerenciadorTransacao.ObterIDAtual();

                    using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
                    {
                        bancoDeDados.IniciarTransacao();

                        credenciado = ImportarPessoas(credenciado, pessoasRelacionadas, bancoDeDados);

                        if (!Validacao.EhValido)
                        {
                            bancoDeDados.Rollback();
                            return requerimento;
                        }

                        using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(UsuarioCredenciado))
                        {
                            bancoDeDadosCredenciado.IniciarTransacao();

                            if (credenciado.Empreendimento.Id > 0)
                            {
                                credenciado = ImportarEmpreendimento(credenciado, bancoDeDados, bancoDeDadosCredenciado);
                            }

                            if (!Validacao.EhValido)
                            {
                                bancoDeDadosCredenciado.Rollback();
                                bancoDeDados.Rollback();
                                return requerimento;
                            }

                            _da.Importar(credenciado, bancoDeDados);

                            _busProjetoDigitalCredenciado.AlterarEtapaTemporario(requerimento.ProjetoDigitalId, eProjetoDigitalEtapaImportacao.Caracterizacao, bancoDeDados);

                            if (!Validacao.EhValido)
                            {
                                bancoDeDadosCredenciado.Rollback();
                                bancoDeDados.Rollback();
                                return requerimento;
                            }

                            busRequerimentoCredenciado.AlterarSituacao(new Requerimento() { Id = credenciado.Id, SituacaoId = (int)eRequerimentoSituacao.Importado }, bancoDeDadosCredenciado);

                            AlterarSituacao(credenciado.Id, eProjetoDigitalSituacao.AguardandoProtocolo, bancoDeDadosCredenciado);

                            if (!Validacao.EhValido)
                            {
                                bancoDeDadosCredenciado.Rollback();
                                bancoDeDados.Rollback();
                                return requerimento;
                            }

                            bancoDeDadosCredenciado.Commit();
                        }

                        bancoDeDados.Commit();
                        Validacao.Add(Mensagem.ProjetoDigital.RequerimentoImportado(requerimento.Numero.ToString()));
                    }
                }
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return requerimento;
        }

        public Requerimento ImportarPessoas(Requerimento requerimento, List<Pessoa> pessoasRelacionadas, BancoDeDados bancoInterno)
        {
            try
            {
                #region Fisica
                Pessoa aux;

                List<Pessoa> pessoas = requerimento.Pessoas.Where(x => x.IsFisica && pessoasRelacionadas.Exists(z => z.CPFCNPJ == x.CPFCNPJ)).ToList();

                pessoas.Where(x => x.SelecaoTipo == (int)eExecutorTipo.Credenciado).ToList().ForEach(r =>
                {
                    _busPessoa.AlterarConjuge(_busPessoa.ObterId(r.CPFCNPJ), bancoInterno);
                });

                pessoas.Where(x => x.SelecaoTipo == (int)eExecutorTipo.Credenciado).ToList().ForEach(r =>
                {
                    aux = _busPessoa.Importar(r, bancoInterno, true);
                    r.InternoId = aux.InternoId;
                });

                pessoas.Where(x => x.SelecaoTipo == (int)eExecutorTipo.Interno).ToList().ForEach(r =>
                {
                    r.InternoId = _busPessoa.ObterId(r.CPFCNPJ, bancoInterno);
                });

                #endregion

                #region Juridica

                List<Pessoa> juridicas = requerimento.Pessoas.Where(x => x.IsJuridica && pessoasRelacionadas.Exists(z => z.CPFCNPJ == x.CPFCNPJ)).ToList();

                juridicas.SelectMany(x => x.Juridica.Representantes).ToList().ForEach(r =>
                {
                    aux = pessoas.FirstOrDefault(y => y.CPFCNPJ == r.CPFCNPJ);
                    if (aux != null && aux.InternoId.HasValue)
                    {
                        r.Id = aux.InternoId.GetValueOrDefault();
                    }
                });

                juridicas.Where(x => x.SelecaoTipo == (int)eExecutorTipo.Credenciado).ToList().ForEach(r =>
                {
                    aux = _busPessoa.Importar(r, bancoInterno);
                    r.InternoId = aux.InternoId;
                });

                juridicas.Where(x => x.SelecaoTipo == (int)eExecutorTipo.Interno).ToList().ForEach(r =>
                {
                    r.InternoId = _busPessoa.ObterId(r.CPFCNPJ, bancoInterno);
                });

                #endregion

                pessoas.AddRange(juridicas);

                if (pessoas != null && pessoas.Count > 0)
                {
                    pessoas.Where(x => x.IsFisica && x.Fisica.ConjugeId > 0).ToList().ForEach(r =>
                    {
                        aux = pessoas.FirstOrDefault(y => y.CPFCNPJ == r.Fisica.ConjugeCPF);

                        if (aux != null)
                        {
                            r.Fisica.ConjugeId = aux.InternoId.GetValueOrDefault();
                            _busPessoa.AlterarConjugeEstadoCivil(r.InternoId.GetValueOrDefault(), r.Fisica.ConjugeId.Value, bancoInterno);
                        }
                    });

                    requerimento.Interessado.Id = pessoas.Where(y => y.Id == requerimento.Interessado.Id).FirstOrDefault().InternoId.GetValueOrDefault();

                    requerimento.Responsaveis.Where(x => x.Representantes != null).SelectMany(z => z.Representantes).ToList().ForEach(r =>
                    {
                        r.Id = pessoas.Where(y => y.Id == r.Id).FirstOrDefault().InternoId.GetValueOrDefault();
                    });

                    requerimento.Responsaveis.ForEach(r =>
                    {
                        aux = pessoas.FirstOrDefault(y => y.CPFCNPJ == r.CpfCnpj);
                        if (aux != null && aux.InternoId.HasValue)
                        {
                            r.Id = aux.InternoId.GetValueOrDefault();
                        }
                    });
                }

                requerimento.Pessoas = pessoas;
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return requerimento;
        }

        public Requerimento ImportarEmpreendimento(Requerimento requerimento, BancoDeDados bancoInterno, BancoDeDados bancoCredenciado)
        {
            try
            {
                requerimento = _empBus.Importar(requerimento, bancoInterno, bancoCredenciado);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }
            return requerimento;
        }

        public List<Pessoa> ObterPessoasRelacionadas(Requerimento requerimento)
        {
            List<Pessoa> pessoasRelacionadas = new List<Pessoa>();

            List<string> cpfCnpj = requerimento.Responsaveis
                .Where(x => x.Representantes != null)
                .SelectMany(x => x.Representantes)
                .Select(x => x.CPFCNPJ)
                .ToList();

            pessoasRelacionadas.AddRange(requerimento.Pessoas);
            
            //TODOS OS REPRESENTANTES PF DOS RESPONSAVEIS
            pessoasRelacionadas.AddRange(requerimento.Pessoas.Where(x => cpfCnpj.Exists(y => x.IsFisica && y == x.CPFCNPJ)));

            //INTERESSADO
            pessoasRelacionadas.Add(requerimento.Pessoas.Where(x => x.CPFCNPJ == requerimento.Interessado.CPFCNPJ).FirstOrDefault());

            //TODOS OS RESPONSAVEIS
            requerimento.Responsaveis.ForEach(r =>
            {
                pessoasRelacionadas.Add(requerimento.Pessoas.Where(x => x.CPFCNPJ == r.CpfCnpj).FirstOrDefault());
            });

            //TODOS OS RESPONSAVEIS DO EMPREEENDIMENTO
            //if (requerimento.Empreendimento.SelecaoTipo == (int)eExecutorTipo.Credenciado && requerimento.Empreendimento.Id > 0)
            //{
            //    EmpreendimentoCredenciadoBus empreendimentoCredenciadoBus = new EmpreendimentoCredenciadoBus();
            //    pessoasRelacionadas.AddRange(empreendimentoCredenciadoBus.ObterResponsaveis(requerimento.Empreendimento.Id));
            //}

            //TODOS OS REPRESENTANTES DOS RESPONSAVEIS DO EMPREEENDIMENTO
            if (requerimento.Empreendimento.SelecaoTipo == (int)eExecutorTipo.Credenciado && requerimento.Empreendimento.Id > 0)
            {
                PessoaCredenciadoBus pessoaCredenciadoBus = new PessoaCredenciadoBus(); 
                EmpreendimentoCredenciadoBus empreendimentoCredenciadoBus = new EmpreendimentoCredenciadoBus();
                var responsaveis = empreendimentoCredenciadoBus.ObterResponsaveis(requerimento.Empreendimento.Id);
                pessoasRelacionadas.AddRange(responsaveis);

                foreach (var responsavel in responsaveis)
                {
                    var pessoa = pessoaCredenciadoBus.Obter(responsavel.Id);
                    if (pessoa.IsJuridica)
                    {
                        foreach (var repre in pessoa.Juridica.Representantes)
                        {
                            pessoasRelacionadas.Add(pessoaCredenciadoBus.Obter(repre.Id));
                        }
                    }
                }
            }

            //Conjuge
            foreach (var item in requerimento.Pessoas.Where(x => x.Fisica.ConjugeId > 0).ToList())
            {
                if (item.SelecaoTipo == (int)eExecutorTipo.Credenciado)
                {
                    pessoasRelacionadas.Add(requerimento.Pessoas.FirstOrDefault(x => x.CPFCNPJ == item.Fisica.ConjugeCPF));
                }
            }

            //Agrupando
            pessoasRelacionadas = pessoasRelacionadas.Where(x => x != null).GroupBy(x => x.CPFCNPJ).Select(y => new Pessoa
            {
                Id = y.First().Id,
                Tipo = y.First().Tipo,
                InternoId = y.First().InternoId,
                NomeRazaoSocial = y.First().NomeRazaoSocial,
                CPFCNPJ = y.First().CPFCNPJ,
                Fisica = y.First().Fisica,
            }).ToList();

            return pessoasRelacionadas;
        }

        internal bool AlterarSituacao(int requerimentoId, eProjetoDigitalSituacao eProjetoDigitalSituacao, BancoDeDados banco = null)
        {
            try
            {
                ProjetoDigital projetoDigital = _busProjetoDigitalCredenciado.Obter(idRequerimento: requerimentoId, banco: banco);
                projetoDigital.Situacao = (int)eProjetoDigitalSituacao;

                return _busProjetoDigitalCredenciado.AlterarSituacao(projetoDigital, banco: banco);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return false;
        }

        public void Editar(ProjetoDigital projeto)
        {
            try
            {
                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
                {
                    _busProjetoDigitalCredenciado.Salvar(projeto, bancoDeDados);
                }
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }
        }

        public void Recusar(ProjetoDigital projeto, BancoDeDados banco = null)
        {
            try
            {
                if (!_validar.Recusar(projeto))
                {
                    return;
                }

                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
                {
                    bancoDeDados.IniciarTransacao();

                    if (_busProjetoDigitalCredenciado.Recusar(projeto, bancoDeDados))
                    {
                        _busProjetoDigitalCredenciado.ExcluirTemporario(projeto.Id);

                        bancoDeDados.Commit();
                    }

                    Validacao.Add(Mensagem.ProjetoDigital.RecusadoSucesso);
                }
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }
        }

        #endregion

        #region Obter/Filtrar

        public Requerimento GerarObjeto(Requerimento requerimento)
        {
            Requerimento aux = Obter(requerimento.Id);

            if (requerimento.Empreendimento.SelecaoTipo == (int)eExecutorTipo.Interno)
            {
                aux.Empreendimento = _empBus.ObterSimplificado(aux.Empreendimento.InternoId.GetValueOrDefault());
            }

            if (requerimento.Interessado.SelecaoTipo == (int)eExecutorTipo.Interno)
            {
                aux.Interessado = _busPessoa.Obter(aux.Interessado.CPFCNPJ, simplificado: true);
            }

            foreach (var responsavel in aux.Responsaveis)
            {
                Pessoa pessoa = requerimento.Pessoas.FirstOrDefault(x => x.CPFCNPJ == responsavel.CpfCnpj);

                if (pessoa != null && pessoa.SelecaoTipo == (int)eExecutorTipo.Interno)
                {
                    Pessoa pessoaAux = _busPessoa.Obter(pessoa.CPFCNPJ, simplificado: true);
                    responsavel.NomeRazao = pessoaAux.NomeRazaoSocial;
                }
            }

            return aux;
        }

        public Resultados<Requerimento> Filtrar(RequerimentoListarFiltro filtrosListar, Paginacao paginacao)
        {
            try
            {
                RequerimentoCredenciadoBus busRequerimentoCredenciado = new RequerimentoCredenciadoBus();
                Resultados<Requerimento> resultados = busRequerimentoCredenciado.Filtrar(filtrosListar, paginacao);

                if (resultados.Quantidade < 1)
                {
                    Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
                }

                return resultados;
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return null;
        }

        public Requerimento Obter(int requerimentoId)
        {
            Requerimento requerimento = null;

            try
            {
                RequerimentoCredenciadoBus busRequerimentoCredenciado = new RequerimentoCredenciadoBus();
                requerimento = busRequerimentoCredenciado.Obter(requerimentoId, true);
                requerimento.ProjetoDigital = _busProjetoDigitalCredenciado.Obter(idRequerimento: requerimentoId);
                requerimento.ProjetoDigitalId = requerimento.ProjetoDigital.Id;
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return requerimento;
        }

        #endregion
    }
}