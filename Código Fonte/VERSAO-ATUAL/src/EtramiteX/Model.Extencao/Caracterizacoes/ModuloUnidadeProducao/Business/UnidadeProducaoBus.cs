using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Data;
using Cred = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business
{
	public class UnidadeProducaoBus : ICaracterizacaoBus
	{
		#region Propriedades

		ConfiguracaoCoordenada _configCoord = new ConfiguracaoCoordenada();
		UnidadeProducaoValidar _validar = null;
		UnidadeProducaoDa _da = new UnidadeProducaoDa();
		ProjetoGeograficoBus _projetoGeoBus = new ProjetoGeograficoBus();
		CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.UnidadeProducao
				};
			}
		}

		public UnidadeProducaoBus()
		{
			_validar = new UnidadeProducaoValidar();
		}

		#region Comandos DML

        public String GetGeoInfo(Coordenada coordenada)
        {
            RequestJson requestJson = new RequestJson();
            ResponseJsonData<dynamic> resposta = new ResponseJsonData<dynamic>();

            String webServiceHost = _configCoord.Obter<String>(ConfiguracaoCoordenada.KeyUrlObterMunicipioCoordenada);
            String webServiceUri = new StringBuilder()
                .Append(webServiceHost)
                .Append("?easting=")
                .Append(coordenada.EastingUtm)
                .Append("&northing=")
                .Append(coordenada.NorthingUtm)
                .ToString();

            resposta = requestJson.Executar<dynamic>(webServiceUri);

            return resposta.Data;
        }

        public bool Salvar(UnidadeProducao caracterizacao)
		{
			try
			{
                if (!_validar.Salvar(caracterizacao))
                {
                    return Validacao.EhValido;
                }

				#region Configurar Salvar

                Endereco endereco = _da.ObterEndereco(caracterizacao.Empreendimento.Id);
                Municipio municipio = _da.ObterMunicipio(endereco.MunicipioId);

				if (!caracterizacao.PossuiCodigoPropriedade)
				{
					if (caracterizacao.Id < 1)
					{
                        int sequencial = _da.ObterSequenciaCodigoPropriedade();

                        caracterizacao.CodigoPropriedade = UnidadeProducaoGenerator.GerarCodigoPropriedade(municipio.Ibge, sequencial);
					}
					else
					 {
						caracterizacao.CodigoPropriedade = ObterPorEmpreendimento(caracterizacao.Empreendimento.Id, true).CodigoPropriedade;
					}
				}

				RequestJson requestJson = new RequestJson();
				ResponseJsonData<dynamic> resposta = new ResponseJsonData<dynamic>();
				UnidadeProducao caracterizacaoBanco = ObterPorEmpreendimento(caracterizacao.Empreendimento.Id);

				int ultimoCodigoUP = _da.ObterUltimoCodigoUP(caracterizacao.Empreendimento.Id);
				foreach (long item in caracterizacao.UnidadesProducao.Where(x => x.PossuiCodigoUP).Select(x => x.CodigoUP))
				{
					int aux = Convert.ToInt32(item.ToString().Substring(14));
					if (aux > ultimoCodigoUP)
					{
						ultimoCodigoUP = aux;
					}
				}

				foreach (UnidadeProducaoItem item in caracterizacao.UnidadesProducao)
				{
					int codigoIbge = 0;
					resposta = requestJson.Executar<dynamic>(_configCoord.Obter<String>(ConfiguracaoCoordenada.KeyUrlObterMunicipioCoordenada) + "?easting=" + item.Coordenada.EastingUtm + "&northing=" + item.Coordenada.NorthingUtm);

					if (resposta.Erros != null && resposta.Erros.Count > 0)
					{
						Validacao.Erros.AddRange(resposta.Erros);
						return Validacao.EhValido;
					}

					var objJson = resposta.Data;
					if (objJson["EstaNoEstado"] && (objJson["Municipio"] == null || Convert.ToInt32(objJson["Municipio"]["IBGE"] ?? 0) == 0))
					{
						Validacao.Add(Mensagem.Mapas.MunicipioSemRetorno);
					}

					if (!Validacao.EhValido)
					{
						return Validacao.EhValido;
					}

					if (objJson["Municipio"] != null)
					{
						codigoIbge = Convert.ToInt32(objJson["Municipio"]["IBGE"] ?? 0);
					}

					ListaValoresDa listaValoresDa = new ListaValoresDa();
					item.Municipio = listaValoresDa.ObterMunicipio(codigoIbge);

					if (!item.PossuiCodigoUP)
					{
						item.AnoAbertura = DateTime.Today.Year.ToString().Substring(2);

						if (item.Id < 1)
						{
							ultimoCodigoUP++;

                            item.CodigoUP = UnidadeProducaoGenerator.GerarCodigoUnidadeProducao(
                                item.Municipio.Ibge
                            ,   caracterizacao.CodigoPropriedade
                            ,   item.AnoAbertura
                            ,   ultimoCodigoUP
                            );
						}
						else
						{
							item.CodigoUP = caracterizacaoBanco.UnidadesProducao.Single(x => x.Id == item.Id).CodigoUP;
						}
					}
					else
					{
						item.AnoAbertura = item.CodigoUP.ToString().Substring(11, 2);
					}

					foreach (var aux in item.ResponsaveisTecnicos)
					{
						aux.CFONumero = aux.CFONumero.Split('-').GetValue(0).ToString();
					}
				}

				#endregion

				if (caracterizacao.UnidadesProducao.Any(x => caracterizacao.UnidadesProducao.Count(y => y.CodigoUP == x.CodigoUP) > 1))
				{
					Validacao.Add(Mensagem.UnidadeProducao.UnidadeProducaoItemIncorreto);
					return false;
				}

				foreach (var item in caracterizacao.UnidadesProducao)
				{
                    if (!UnidadeProducaoGenerator.CodigoUpHasCodigoPropriedade(caracterizacao.CodigoPropriedade, item.CodigoUP))
					{
						Validacao.Add(Mensagem.UnidadeProducao.CodigoUPNaoContemCodPropriedade(item.CodigoUP));
						return false;
					}
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(caracterizacao, bancoDeDados);

					Validacao.Add(Mensagem.UnidadeProducao.SalvoSucesso);

					bancoDeDados.Commit();
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool Excluir(int empreendimento, BancoDeDados banco = null, bool validarDependencias = true)
		{
			try
			{
				if (!_caracterizacaoValidar.Basicas(empreendimento))
				{
					return Validacao.EhValido;
				}

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.UnidadeProducao))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					Validacao.Add(Mensagem.UnidadeProducao.ExcluidoSucesso);

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter

		public UnidadeProducao ObterPorEmpreendimento(int EmpreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			UnidadeProducao caracterizacao = null;
			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(EmpreendimentoId, simplificado: simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			return _da.ObterDadosPdfTitulo(empreendimento, atividade, banco);
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			//Para o caso da coluna da atividade estar na tabela principal
			return _busCaracterizacao.ObterAtividades(empreendimento, Caracterizacao.Tipo);
		}

		public UnidadeProducaoItem ObterUnidadeProducaoItem(int id)
		{
			UnidadeProducaoItem item = null;
			try
			{
				item = _da.ObterUnidadeProducaoItem(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return item;
		}

		#endregion

		public bool CopiarDadosCredenciado(Dependencia dependencia, int empreendimentoInternoId, BancoDeDados banco, BancoDeDados bancoCredenciado = null)
		{
			if (banco == null)
			{
				return false;
			}

			if (_validar == null)
			{
				_validar = new UnidadeProducaoValidar();
			}

			#region Configurar Unidade de Produção

			Cred.UnidadeProducaoBus credenciadoBus = new Cred.UnidadeProducaoBus();
			UnidadeProducao caracterizacao = credenciadoBus.ObterHistorico(dependencia.DependenciaId, dependencia.DependenciaTid);

			caracterizacao.Empreendimento.Id = empreendimentoInternoId;
			caracterizacao.CredenciadoID = caracterizacao.Id;
			caracterizacao.Id = 0;
			caracterizacao.Tid = string.Empty;
			caracterizacao.UnidadesProducao.SelectMany(u => u.ResponsaveisTecnicos).ToList().ForEach(r => { r.IdRelacionamento = 0; });
			caracterizacao.UnidadesProducao.SelectMany(u => u.Produtores).ToList().ForEach(p => {
				p.Id = _busCaracterizacao.ObterPessoaID(p.CpfCnpj, banco);
				p.IdRelacionamento = 0; 
			});

			#endregion

			if (_validar.CopiarDadosCredenciado(caracterizacao))
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					//Setar ID 
					caracterizacao.Id = ObterPorEmpreendimento(empreendimentoInternoId, simplificado: true, banco: bancoDeDados).Id;

					_da.CopiarDadosCredenciado(caracterizacao, bancoDeDados);

					credenciadoBus.AtualizarInternoIdTid(caracterizacao.CredenciadoID, caracterizacao.Id, GerenciadorTransacao.ObterIDAtual(), bancoCredenciado);

					bancoDeDados.Commit();
				}
			}

			return Validacao.EhValido;
		}
	}
}