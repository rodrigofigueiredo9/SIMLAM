using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business
{
	public class EmpreendimentoBus
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		EmpreendimentoDa _da = null;

		EmpreendimentoValidar _validar = null;
		EmpreendimentoMsg Msg = new EmpreendimentoMsg();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoCoordenada> _configCoordenada = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());
		GerenciadorConfiguracao<ConfiguracaoEmpreendimento> _configEmpreendimento = new GerenciadorConfiguracao<ConfiguracaoEmpreendimento>(new ConfiguracaoEmpreendimento());

		public String UsuarioGeo
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		#endregion Propriedades

		#region Ações de DML

		public EmpreendimentoBus()
		{
			_validar = new EmpreendimentoValidar();
			_da = new EmpreendimentoDa(strBancoDeDadosGeo: UsuarioGeo);
		}

		public EmpreendimentoBus(EmpreendimentoValidar validacao)
		{
			_validar = validacao;
			_da = new EmpreendimentoDa(strBancoDeDadosGeo: UsuarioGeo);
		}

		public bool Salvar(Empreendimento empreendimento)
		{
			try
			{
				if (_validar.Salvar(empreendimento))
				{
					Mensagem msg = VerificarLocalizacaoEmpreendimento(empreendimento.Coordenada.EastingUtmTexto, empreendimento.Coordenada.NorthingUtmTexto, empreendimento.Enderecos[0].EstadoId, empreendimento.Enderecos[0].MunicipioId);
					if (msg.Texto != null)
					{
						Validacao.Add(msg);
						return Validacao.EhValido;
					}

					if (empreendimento.Coordenada.Tipo.Id > 0)
					{
						empreendimento.Coordenada.Datum.Sigla = _busLista.Datuns.SingleOrDefault(x => Equals(x.Id, empreendimento.Coordenada.Datum.Id)).Sigla;
					}

					#region Responsáveis

					if (empreendimento.Id <= 0)
					{
						empreendimento.Responsaveis.ForEach(r =>
						{
							r.OrigemTexto = "Inserido pelo IDAF";
						});
					}
					else
					{
						Empreendimento empHistorico = ObterHistorico(empreendimento.Id, empreendimento.Tid);

						List<Responsavel> responsaveis = empreendimento.Responsaveis.Where(x => !empHistorico.Responsaveis.Exists(y => y.CpfCnpj == x.CpfCnpj)).ToList();
						responsaveis.ForEach(r =>
						{
							r.OrigemTexto = "Inserido pelo IDAF";
						});

						empreendimento.Responsaveis.ForEach(r =>
						{
							Responsavel resp = empHistorico.Responsaveis.FirstOrDefault(x => x.CpfCnpj == r.CpfCnpj);

							if (resp != null && r.Tipo != resp.Tipo)
							{
								r.OrigemTexto = "Alterado pelo IDAF";
							}
						});

						empreendimento.Responsaveis.Where(x => string.IsNullOrEmpty(x.OrigemTexto)).ToList().ForEach(r =>
						{
							Responsavel resp = empHistorico.Responsaveis.FirstOrDefault(x => x.CpfCnpj == r.CpfCnpj);
							r.OrigemTexto = resp.OrigemTexto;
						});
					}

					#endregion Responsáveis

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(empreendimento, bancoDeDados);

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

		public bool Excluir(int id)
		{
			try
			{
				if (_validar.Excluir(id))
				{
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
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

        

		public Requerimento Importar(Requerimento requerimento, BancoDeDados bancoInterno, BancoDeDados bancoCredenciado)
		{
			try
			{
				int id = requerimento.Empreendimento.Id;
				EmpreendimentoCredenciadoBus bus = new EmpreendimentoCredenciadoBus();
				Empreendimento empCredenciado = bus.Obter(requerimento.Empreendimento.Id); //Busca o empreendimento no banco do credenciado
				Empreendimento empInterno = null;
				empCredenciado.SelecaoTipo = requerimento.Empreendimento.SelecaoTipo;

				#region Empreendimento Cadastrado no Interno

				empCredenciado.Id = 0;

                if (requerimento == null || requerimento.Empreendimento == null)
                {
                    try
                    {
                        empCredenciado = bus.ObterNovoEmpreendimento(requerimento.Empreendimento.Id); //Faça nova busca pelo credenciado
                    }
                    catch (Exception ex)
                    {
                        empCredenciado = bus.Obter(requerimento.Empreendimento.Id);
                    }
                }

				if (empCredenciado.InternoId.GetValueOrDefault() > 0)
				{
					empInterno = Obter(empCredenciado.InternoId.GetValueOrDefault());

					if (empInterno != null && empInterno.Id > 0)
					{
						empCredenciado.Id = empInterno.Id;
						empCredenciado.Coordenada.Id = empInterno.Coordenada.Id;
					}
				}

				if (!string.IsNullOrEmpty(empCredenciado.CNPJ) && empCredenciado.Id <= 0)
				{
					empCredenciado.Id = _da.ObterId(empCredenciado.CNPJ);
				}

				#endregion Empreendimento Cadastrado no Interno

				#region Responsáveis

				empCredenciado.Responsaveis.ForEach(r =>
				{
					r.Id = requerimento.Pessoas.Where(x => x.Id == r.Id).SingleOrDefault().InternoId.GetValueOrDefault();
				});

				ConfigurarResponsaveis(empCredenciado, empInterno);

				#endregion Responsáveis

				#region Apagar ID's

				empCredenciado.Responsaveis.ForEach(r => { r.IdRelacionamento = 0; });
				empCredenciado.Enderecos.ForEach(r => { r.Id = 0; });
				empCredenciado.MeiosContatos.ForEach(r => { r.Id = 0; });
				empCredenciado.Coordenada.Datum.Sigla = ((_configCoordenada.Obter<List<Datum>>(ConfiguracaoCoordenada.KeyDatuns).FirstOrDefault(x => x.Id == empCredenciado.Coordenada.Datum.Id)) ?? new Datum()).Texto;

				#endregion Apagar ID's

				//Verificar CEP
				if (!_validar.Salvar(empCredenciado, true))
				{
					return null;
				}

				if (empCredenciado.Id == 0)
				{
					_da.Criar(empCredenciado, bancoInterno);
				}
				else
				{
					_da.Editar(empCredenciado, bancoInterno);
				}

				empCredenciado.InternoId = empCredenciado.Id;
				requerimento.Empreendimento = empCredenciado;
				bus.SalvarInternoId(new Empreendimento() { Id = id, InternoId = empCredenciado.Id, InternoTid = empCredenciado.Tid, Codigo = empCredenciado.Codigo }, bancoCredenciado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return requerimento;
		}

		public void ConfigurarResponsaveis(Empreendimento empCredenciado, Empreendimento empInterno = null)
		{
			if (empInterno == null)
			{
				if (empCredenciado.InternoId.GetValueOrDefault() > 0)
				{
					empInterno = Obter(empCredenciado.InternoId.GetValueOrDefault());
				}
				else
				{
					empInterno = new Empreendimento();
				}
			}

			CredenciadoPessoa credenciado = new CredenciadoBus().Obter(empCredenciado.CredenciadoId.GetValueOrDefault());

			List<Responsavel> responsaveis = empCredenciado.Responsaveis.Where(x => !empInterno.Responsaveis.Exists(y => y.CpfCnpj == x.CpfCnpj)).ToList();
			responsaveis.ForEach(r =>
			{
				r.CredenciadoUsuarioId = credenciado.Id;
				r.Origem = 1;
				r.OrigemTexto = "Inserido pelo Credenciado • Perfil: " + credenciado.TipoTexto;
			});

			empCredenciado.Responsaveis.ForEach(r =>
			{
				if (empInterno != null && empInterno.Id > 0)
				{
					Responsavel resp = empInterno.Responsaveis.FirstOrDefault(x => x.CpfCnpj == r.CpfCnpj);

					if (resp != null && r.Tipo != resp.Tipo)
					{
						r.CredenciadoUsuarioId = credenciado.Id;
						r.Origem = 1;
						r.OrigemTexto = "Alterado pelo Credenciado • Perfil: " + credenciado.TipoTexto;
					}
				}
			});

			if (empInterno != null && empInterno.Id > 0 && empCredenciado.InternoTid != null)
			{
				Empreendimento empHistorico = ObterHistorico(empCredenciado.InternoId.GetValueOrDefault(), empCredenciado.InternoTid);

				responsaveis = empInterno.Responsaveis.Where(x =>
					!empCredenciado.Responsaveis.Exists(y => y.CpfCnpj == x.CpfCnpj) &&
					empHistorico.Responsaveis.Exists(y => y.CpfCnpj == x.CpfCnpj)).ToList();
				responsaveis.ForEach(r =>
				{
					r.CredenciadoUsuarioId = credenciado.Id;
					r.Origem = 1;
					r.OrigemTexto = "Removido pelo Credenciado • Perfil: " + credenciado.TipoTexto;
				});
				empCredenciado.Responsaveis.AddRange(responsaveis);

				empCredenciado.Responsaveis.Where(x => string.IsNullOrEmpty(x.OrigemTexto)).ToList().ForEach(r =>
				{
					Responsavel resp = empHistorico.Responsaveis.FirstOrDefault(x => x.CpfCnpj == r.CpfCnpj) ?? new Responsavel();
					r.OrigemTexto = resp.OrigemTexto;
				});
			}
		}

		#endregion

		#region Obter / Filtrar

		public List<Pessoa> ObterEmpreendimentoResponsaveis(int empreendimento)
		{
			try
			{
				return _da.ObterEmpreendimentoResponsaveis(empreendimento);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new List<Pessoa>();
		}

		public List<EmpreendimentoAtividade> Atividades
		{
			get { return _da.Atividades(); }
		}

		public Resultados<Empreendimento> Filtrar(ListarEmpreendimentoFiltro filtrosListar, Paginacao paginacao, bool validarEncontrouRegistros = true)
		{
			try
			{
				if (!string.IsNullOrEmpty(filtrosListar.AreaAbrangencia))
				{
					filtrosListar.Coordenada.Datum.Sigla = _busLista.Datuns.SingleOrDefault(x => Equals(x.Id, filtrosListar.Coordenada.Datum.Id)).Sigla;
				}

				Filtro<ListarEmpreendimentoFiltro> filtro = new Filtro<ListarEmpreendimentoFiltro>(filtrosListar, paginacao);
				Resultados<Empreendimento> resultados = _da.Filtrar(filtro);

				if (validarEncontrouRegistros && resultados.Quantidade <= 0)
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

		public Empreendimento Obter(int id)
		{
			try
			{
				Empreendimento emp = _da.Obter(id);

				if (emp.Id == 0)
				{
					Validacao.Add(Mensagem.Empreendimento.Inexistente);
				}

				return emp;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Empreendimento ObterSimplificado(int id)
		{
			try
			{
				Empreendimento emp = _da.ObterSimplificado(id);

				if (emp == null)
				{
					Validacao.Add(Mensagem.Empreendimento.Inexistente);
				}

				return emp;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Endereco ObterEndereco(int id)
		{
			try
			{
				return _da.ObterEndereco(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<PessoaLst> ObterResponsaveis(int id)
		{
			try
			{
				return _da.ObterResponsaveis(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<PessoaLst> ObterResponsaveisComTipo(int id)
		{
			try
			{
				return _da.ObterResponsaveisComTipo(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<PessoaLst> ObterListaInteressadoEmpreendimento(int empreendimento, int requerimento)
		{
			try
			{
				return _da.ObterListaInteressadoEmpreendimento(empreendimento, requerimento);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<PessoaLst> ObterListaResponsavelTecnicoEmpreendimento(int empreendimento, int requerimento)
		{
			try
			{
				return _da.ObterListaResponsavelTecnicoEmpreendimento(empreendimento, requerimento);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public int ObterEnderecoZona(int id)
		{
			try
			{
				return _da.ObterEnderecoZona(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		public Empreendimento ObterHistorico(int id, string tid)
		{
			try
			{
				Empreendimento emp = _da.ObterHistorico(id, tid);

				//if (emp.Id == 0)
				//{
				//    Validacao.Add(Mensagem.Empreendimento.Inexistente);
				//}

				return emp;
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


		#endregion

		#region Verificar / Validar

		public bool ExisteEmpreendimentoCnpj(string cnpj, int? id = null)
		{
			return _da.ExisteCnpj(cnpj, id);
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

		public void ValidarLocalizarFiscalizacao(ListarEmpreendimentoFiltro filtros)
		{
			try
			{
				_validar.ValidarLocalizarFiscalizacao(filtros);
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

		public int ExisteEmpreendimento(string cnpj, int? id = null)
		{
			return _da.Existe(cnpj, id);
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

		#endregion
	}
}