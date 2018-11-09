using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business
{
	public class EmpreendimentoCredenciadoBus
	{
		#region Propriedades

		EmpreendimentoCredenciadoDa _da = null;
		EmpreendimentoCredenciadoValidar _validar = null;
		EmpreendimentoMsg Msg = new EmpreendimentoMsg();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		GerenciadorConfiguracao<ConfiguracaoCoordenada> _configCoordenada;
		GerenciadorConfiguracao<ConfiguracaoEmpreendimento> _configEmpreendimento;
		GerenciadorConfiguracao<ConfiguracaoEndereco> _configEnd;
		RequerimentoCredenciadoBus _busRequerimento;

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public EmpreendimentoCredenciadoBus() : this(new EmpreendimentoCredenciadoValidar()) { }

		public EmpreendimentoCredenciadoBus(EmpreendimentoCredenciadoValidar validacao)
		{
			_validar = validacao;
			Msg = new EmpreendimentoMsg();
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_configCoordenada = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());
			_configEmpreendimento = new GerenciadorConfiguracao<ConfiguracaoEmpreendimento>(new ConfiguracaoEmpreendimento());
			_configEnd = new GerenciadorConfiguracao<ConfiguracaoEndereco>(new ConfiguracaoEndereco());
			_da = new EmpreendimentoCredenciadoDa();
			_busRequerimento = new RequerimentoCredenciadoBus();
		}

		#region Ações de DML

		public bool Salvar(Empreendimento empreendimento)
		{
			try
			{
				if (_validar.Salvar(empreendimento))
				{
					Mensagem erros = VerificarLocalizacaoEmpreendimento(empreendimento.Coordenada.EastingUtmTexto, empreendimento.Coordenada.NorthingUtmTexto, empreendimento.Enderecos[0].EstadoId, empreendimento.Enderecos[0].MunicipioId);
					if (erros.Texto != null)
					{
						Validacao.Add(erros);
						return Validacao.EhValido;
					}

					empreendimento.CredenciadoId = User.FuncionarioId;

					if (empreendimento.InternoId.GetValueOrDefault() > 0)
					{
						empreendimento.InternoTid = new EmpreendimentoInternoBus().ObterSimplificado(empreendimento.InternoId.Value).Tid;
					}

					if (empreendimento.Coordenada.Tipo.Id > 0)
					{
						empreendimento.Coordenada.Datum.Sigla = ListaCredenciadoBus.Datuns.SingleOrDefault(x => Equals(x.Id, empreendimento.Coordenada.Datum.Id)).Sigla;
					}

					#region Utilizar o mesmo endereço de localização

					if (empreendimento.Enderecos.Count == 1)
					{
						Endereco enderecoLocalizacao = empreendimento.Enderecos.First();

						Endereco endereco = new Endereco();
						endereco.Correspondencia = 1;
						endereco.Cep = enderecoLocalizacao.Cep;
						endereco.Logradouro = enderecoLocalizacao.Logradouro;
						endereco.Bairro = enderecoLocalizacao.Bairro;
						endereco.EstadoId = enderecoLocalizacao.EstadoId;
						endereco.MunicipioId = enderecoLocalizacao.MunicipioId;
						endereco.Numero = enderecoLocalizacao.Numero;
						endereco.DistritoLocalizacao = enderecoLocalizacao.DistritoLocalizacao;
						endereco.Complemento = enderecoLocalizacao.Complemento;

						empreendimento.Enderecos.Add(endereco);
					}

					#endregion

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();
						PessoaCredenciadoBus pessoaCredenciadoBus = new PessoaCredenciadoBus();
						Pessoa aux;
						List<Mensagem> mensagens = new List<Mensagem>();

						foreach (Responsavel responsavel in empreendimento.Responsaveis)
						{
							if (responsavel.Id <= 0)
							{
								aux = pessoaCredenciadoBus.ObterPessoa(interno: responsavel.InternoId);
								Pessoa pessoaCredenciado = pessoaCredenciadoBus.Obter(aux.CPFCNPJ, bancoDeDados, simplificado: true, credenciadoId: User.FuncionarioId);

								aux.Id = pessoaCredenciado.Id;
								pessoaCredenciadoBus.Salvar(aux, bancoDeDados);

								responsavel.Id = aux.Id;

								if (!Validacao.EhValido)
								{
									mensagens.Add(Mensagem.Pessoa.DadosRepresentanteIncompleto(aux.NomeRazaoSocial));
								}

							}
						}

						if (!Validacao.EhValido)
						{
							mensagens.ForEach(msg =>
							{
								msg.Texto = msg.Texto.Replace("representante", "responsável");
							});

							Validacao.Erros = mensagens;
							bancoDeDados.Rollback();
							return false;
						}

						_da.Salvar(empreendimento, bancoDeDados);

						#region Solicitação de CAR

						CARSolicitacaoBus carSolicitacaoBus = new CARSolicitacaoBus();
						CARSolicitacao carSolicitacao = new CARSolicitacao();
						carSolicitacao.Empreendimento.Id = empreendimento.Id;
						//carSolicitacaoBus.AlterarSituacao(carSolicitacao, new CARSolicitacao() { SituacaoId = (int)eCARSolicitacaoSituacao.Invalido }, bancoDeDados);

						#endregion

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool SalvarInternoId(Empreendimento empreendimento, BancoDeDados banco = null)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.SalvarInternoId(empreendimento, bancoDeDados);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool Excluir(int id)
		{
			try
			{
				if (_validar.Excluir(id))
				{
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						GerenciadorTransacao.ObterIDAtual();
						bancoDeDados.IniciarTransacao();

						_da.Excluir(id, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Erros.Clear();

						Validacao.Add(Mensagem.Empreendimento.Excluir);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.Erros.RemoveAll(x => x.Tipo == eTipoMensagem.Sucesso);
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter / Filtrar

		public Resultados<Empreendimento> Filtrar(ListarEmpreendimentoFiltro filtrosListar, Paginacao paginacao, bool validarEncontrouRegistros = true)
		{
			try
			{
				filtrosListar.Credenciado = User.FuncionarioId;
				if (!string.IsNullOrEmpty(filtrosListar.AreaAbrangencia))
				{
					filtrosListar.Coordenada.Datum.Sigla = ListaCredenciadoBus.Datuns.SingleOrDefault(x => Equals(x.Id, filtrosListar.Coordenada.Datum.Id)).Sigla;
				}

				Filtro<ListarEmpreendimentoFiltro> filtro = new Filtro<ListarEmpreendimentoFiltro>(filtrosListar, paginacao);
				Resultados<Empreendimento> resultados = _da.Filtrar(filtro);

				if (validarEncontrouRegistros && resultados.Quantidade <= 0)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				foreach (var item in resultados.Itens)
				{
					item.SegmentoTexto = (ListaCredenciadoBus.Segmentos.SingleOrDefault(x => x.Id == item.Segmento.ToString()) ?? new Segmento()).Texto;
				}

				if (filtro.OdenarPor == 1)
				{
					resultados.Itens.OrderBy(x => x.SegmentoTexto);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Empreendimento Obter(int id, bool simplificado = false)
		{
			try
			{
				Empreendimento emp = _da.Obter(id, simplificado: simplificado) ?? new Empreendimento();
				emp.Enderecos = emp.Enderecos ?? new List<Endereco>();

				if (emp != null && emp.Id > 0)
				{
					emp.SegmentoDenominador = (_configEmpreendimento.Obter<List<Segmento>>(ConfiguracaoEmpreendimento.KeySegmentos).First(x => x.Id == emp.Segmento.GetValueOrDefault().ToString()) ?? new Segmento()).Denominador;
					emp.SegmentoTexto = (_configEmpreendimento.Obter<List<Segmento>>(ConfiguracaoEmpreendimento.KeySegmentos).First(x => x.Id == emp.Segmento.GetValueOrDefault().ToString()) ?? new Segmento()).Texto;

					if (!simplificado)
					{
						emp.Coordenada.Tipo.Texto = ((_configCoordenada.Obter<List<CoordenadaTipo>>(ConfiguracaoCoordenada.KeyTiposCoordenada).FirstOrDefault(x => x.Id == emp.Coordenada.Tipo.Id)) ?? new CoordenadaTipo()).Texto;
						emp.Coordenada.Datum.Texto = ((_configCoordenada.Obter<List<Datum>>(ConfiguracaoCoordenada.KeyDatuns).FirstOrDefault(x => x.Id == emp.Coordenada.Datum.Id)) ?? new Datum()).Texto;
						emp.Coordenada.HemisferioUtmTexto = ((_configCoordenada.Obter<List<CoordenadaHemisferio>>(ConfiguracaoCoordenada.KeyHemisferios).FirstOrDefault(x => x.Id == emp.Coordenada.HemisferioUtm)) ?? new CoordenadaHemisferio()).Texto;

						emp.Coordenada.FormaColetaTexto = ((_configCoordenada.Obter<List<Lista>>(ConfiguracaoCoordenada.KeyFormaColetaPonto).FirstOrDefault(x => x.Id == emp.Coordenada.FormaColeta.GetValueOrDefault().ToString())) ?? new Lista()).Texto;
						emp.Coordenada.LocalColetaTexto = ((_configCoordenada.Obter<List<Lista>>(ConfiguracaoCoordenada.KeyLocalColetaPonto).FirstOrDefault(x => x.Id == emp.Coordenada.LocalColeta.GetValueOrDefault().ToString())) ?? new Lista()).Texto;

						foreach (var item in emp.Enderecos)
						{
							if (item.EstadoId > 0)
							{
								item.EstadoTexto = _configEnd.Obter<List<Estado>>(ConfiguracaoEndereco.KeyEstados).First(x => x.Id == item.EstadoId).Texto;
								item.MunicipioTexto = _da.ObterMunicipio(item.MunicipioId).Texto;
							}
						}

						foreach (var item in emp.Responsaveis)
						{
							item.TipoTexto = (_configEmpreendimento.Obter<List<TipoResponsavel>>(ConfiguracaoEmpreendimento.KeyTiposResponsavel).First(x => x.Id == item.Tipo.GetValueOrDefault()) ?? new TipoResponsavel()).Texto;
						}

						if (emp.Atividade.Id > 0)
						{
							AtividadeEmpreendimentoInternoBus atividadeEmpBus = new AtividadeEmpreendimentoInternoBus();
							emp.Atividade.Atividade = atividadeEmpBus.Filtrar(new AtividadeListarFiltro() { Id = emp.Atividade.Id }, new Paginacao() { PaginaAtual = 1, QuantPaginacao = 1 }).Itens[0].Atividade;
						}

						ContatoLst contatoAux = new ContatoLst();
						emp.MeiosContatos.ForEach(x =>
						{
							contatoAux = (_configSys.Obter<List<ContatoLst>>(ConfiguracaoSistema.KeyMeiosContato).SingleOrDefault(y => y.Id == (int)x.TipoContato) ?? new ContatoLst());
							x.Mascara = contatoAux.Mascara;
							x.TipoTexto = contatoAux.Texto;
						});
					}
				}

				return emp;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Empreendimento ObterEmpreendimento(int id, int internoId = 0, bool simplificado = false)
		{
			Empreendimento empreendimento;
			Pessoa aux;

			if (id > 0)
			{
				empreendimento = Obter(id, simplificado);
			}
			else
			{
				EmpreendimentoInternoBus busInterno = new EmpreendimentoInternoBus();

				if (simplificado)
				{
					empreendimento = busInterno.ObterSimplificado(internoId);
				}
				else
				{
					empreendimento = busInterno.Obter(internoId);
				}

				PessoaCredenciadoBus pessoaCredenciadoBus = new PessoaCredenciadoBus();

				foreach (Responsavel responsavel in empreendimento.Responsaveis)
				{
					aux = pessoaCredenciadoBus.Obter(responsavel.CpfCnpj, simplificado: true, credenciadoId: User.FuncionarioId);

					if (aux.Id > 0)
					{
						responsavel.Id = aux.Id;
						responsavel.InternoId = aux.InternoId.GetValueOrDefault();
						responsavel.NomeRazao = aux.NomeRazaoSocial;
					}
					else
					{
						responsavel.InternoId = responsavel.Id.GetValueOrDefault();
						responsavel.Id = 0;
					}
				}

				foreach (var item in empreendimento.Enderecos)
				{
					item.Id = 0;
				}

				empreendimento.InternoId = empreendimento.Id;
				empreendimento.Id = 0;
			}

			return empreendimento;
		}

        public Empreendimento ObterNovoEmpreendimento(int id, int internoId = 0, bool simplificado = false)
        {
            Empreendimento empreendimento;
            Pessoa aux;

            EmpreendimentoInternoBus busInterno = new EmpreendimentoInternoBus();

            empreendimento = busInterno.ObterSimplificado(internoId);

            PessoaCredenciadoBus pessoaCredenciadoBus = new PessoaCredenciadoBus();

            foreach (Responsavel responsavel in empreendimento.Responsaveis)
            {
                aux = pessoaCredenciadoBus.Obter(responsavel.CpfCnpj, simplificado: true, credenciadoId: User.FuncionarioId);

                if (aux.Id > 0)
                {
                    responsavel.Id = aux.Id;
                    responsavel.InternoId = aux.InternoId.GetValueOrDefault();
                    responsavel.NomeRazao = aux.NomeRazaoSocial;
                }
                else
                {
                    responsavel.InternoId = responsavel.Id.GetValueOrDefault();
                    responsavel.Id = 0;
                }
            }

            foreach (var item in empreendimento.Enderecos)
            {
                item.Id = 0;
            }

            empreendimento.InternoId = empreendimento.Id;
            empreendimento.Id = 0;


            return empreendimento;
        }

		public List<EmpreendimentoAtividade> Atividades
		{
			get { return _da.Atividades(); }
		}

		public List<Pessoa> ObterResponsaveis(int empreendimento)
		{
			try
			{
				return _da.ObterResponsaveis(empreendimento);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<PessoaLst> ObterResponsaveisLista(int empreendimento)
		{
			List<Pessoa> responsaveis = ObterResponsaveis(empreendimento);
			List<PessoaLst> retorno = new List<PessoaLst>();

			responsaveis.ForEach(responsavel => { retorno.Add(new PessoaLst() { Id = responsavel.Id, Texto = responsavel.NomeFantasia }); });

			return retorno;
		}

		public Empreendimento Obter(string CNPJ, bool simplificado)
		{
			try
			{
				return _da.Obter(cnpj: CNPJ, simplificado: simplificado);

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public ResponseJsonData<dynamic> ObterEstadosMunicipiosPorCoordenada(String easting, String northing)
		{
			//CrossDomain case
			RequestJson requestJson = new RequestJson();
			ResponseJsonData<dynamic> resposta = new ResponseJsonData<dynamic>();

			try
			{
				resposta = requestJson.Executar<dynamic>(_configCoordenada.Obter<String>(ConfiguracaoCoordenada.KeyUrlObterMunicipioCoordenada) + "?easting=" + easting + "&northing=" + northing);

				if (resposta.Erros != null && resposta.Erros.Count > 0)
				{
					Validacao.Erros.AddRange(resposta.Erros);
				}

				var objJson = resposta.Data;
				if (objJson["EstaNoEstado"] && (objJson["Municipio"] == null || Convert.ToInt32(objJson["Municipio"]["IBGE"] ?? 0) == 0))
				{
					Validacao.Add(Mensagem.Mapas.MunicipioSemRetorno);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return resposta;
		}

		public List<Empreendimento> ObterEmpreendimentoResponsavel (int requerimentoId)
		{
			List<Empreendimento> retorno = new List<Empreendimento>();
			Requerimento requerimento = _busRequerimento.ObterSimplificado(requerimentoId);

			try
			{
				foreach(int emp in _da.ObterEmpreendimentoResponsavel(requerimento.Interessado.Id))
				{
					Empreendimento empreendimento = new Empreendimento();
					empreendimento = ObterEmpreendimento(0, emp);

					retorno.Add(empreendimento);
				}
			}catch(Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return retorno;
		}

		#endregion

		#region Verificar / Validar

		public bool ExisteEmpreendimentoCnpj(string cnpj, int? id = null)
		{
			return _da.ExisteCnpj(cnpj, User.FuncionarioId, id);
		}

		public bool ValidarEmpreendimentoCnpjLocalizar(string cnpj, int? id = null)
		{
			if (!ValidacoesGenericasBus.Cnpj(cnpj))
			{
				Validacao.Add(Mensagem.Empreendimento.CnpjInvalido);
				return Validacao.EhValido;
			}

			bool valor = ExisteEmpreendimentoCnpj(cnpj, id);

			if (!valor)
			{
				Validacao.Add(Mensagem.Empreendimento.CnpjDisponivel);
			}

			return valor;
		}

		public Mensagem ExisteCnpj(string cnpj, int? id = null)
		{
			Mensagem msg = new Mensagem();
			try
			{
				if (ValidacoesGenericasBus.Cnpj(cnpj))
				{
					bool retorno = retorno = ExisteEmpreendimentoCnpj(cnpj, id);

					if (retorno)
					{
						msg = Msg.CnpjJaExistente;
					}
					else
					{
						msg = Msg.CnpjDisponivel;
					}
				}
				else
				{
					msg = Msg.CnpjInvalido;
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return msg;
		}

		public void ValidarLocalizar(ListarEmpreendimentoFiltro filtros)
		{
			try
			{
				_validar.ValidarLocalizar(filtros);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public bool ValidarEmPosse(int id)
		{
			try
			{
				return _validar.EmPosse(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return false;
		}

		public Mensagem VerificarLocalizacaoEmpreendimento(string easting, string northing, int estadoID, int municipioID)
		{
			RequestJson requestJson = new RequestJson();
			ResponseJsonData<dynamic> resposta = new ResponseJsonData<dynamic>();
			Municipio municipioCoordenada = new Municipio();

			resposta = requestJson.Executar<dynamic>(_configCoordenada.Obter<String>(ConfiguracaoCoordenada.KeyUrlObterMunicipioCoordenada) + "?easting=" + easting + "&northing=" + northing);

			if (estadoID != 8 && !Convert.ToBoolean(resposta.Data["EstaNoEstado"]))
			{
				return new Mensagem();
			}

			if (Convert.ToBoolean(resposta.Data["EstaNoEstado"]))
			{
				municipioCoordenada = new ListaValoresDa().ObterMunicipio(Convert.ToInt32(resposta.Data["Municipio"]["IBGE"]));
			}

			if (municipioCoordenada.Id != municipioID)
			{
				return Msg.MunicipioEmpreendimentoDiferenteResponsavel;
			}

			if (estadoID != 8 && Convert.ToBoolean(resposta.Data["EstaNoEstado"]))
			{
				return Msg.MunicipioEmpreendimentoDiferenteResponsavel;
			}

			if (estadoID == 8 && !Convert.ToBoolean(resposta.Data["EstaNoEstado"]))
			{
				return Msg.MunicipioEmpreendimentoDiferenteResponsavel;
			}

			return new Mensagem();
		}

		public bool ExisteEmpreendimentoResponsavel(int pessoa)
		{
			try
			{
				return _da.ObterEmpreendimentoResponsavel(pessoa).Count > 0 ? true : false;
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}
			return false;
		}

		#endregion
	}
}