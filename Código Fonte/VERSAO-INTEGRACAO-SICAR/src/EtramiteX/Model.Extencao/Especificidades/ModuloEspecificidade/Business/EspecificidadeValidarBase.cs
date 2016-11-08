using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business
{
	public class EspecificidadeValidarBase
	{
		EspecificidadeDa _da = new EspecificidadeDa();
		LicencaValidar _validarLicenca = new LicencaValidar();

		public bool RequerimentoAtividade(IEspecificidade especificidade, bool solicitado = true, bool jaAssociado = true, bool faseAnterior = false, bool apenasObrigatoriedade = false, bool atividadeAndamento = true)
		{

			if (especificidade.ProtocoloReq.RequerimentoId <= 0)
			{
				// Msg: O requerimento padrão é obrigatório.
				Validacao.Add(Mensagem.Especificidade.RequerimentoPradroObrigatoria);
			}

			if (especificidade.Atividades == null || especificidade.Atividades.Count == 0 || especificidade.Atividades[0].Id == 0)
			{
				// Msg: A atividade é obrigatória.
				Validacao.Add(Mensagem.Especificidade.AtividadeObrigatoria);
			}

			//Retorna se já encontrou erro de obrigatóriedade
			if (Validacao.Erros.Exists(x => x.Texto == Mensagem.Especificidade.RequerimentoPradroObrigatoria.Texto) || Validacao.Erros.Exists(x => x.Texto == Mensagem.Especificidade.AtividadeObrigatoria.Texto))
			{
				return false;
			}

			//Verifica se o requerimento ainda está no protocolo
			if (!_da.ValidarRequerimentoAssociado(especificidade.ProtocoloReq.RequerimentoId, especificidade.Titulo.Protocolo.Id, especificidade.ProtocoloReq.Id))
			{
				// Msg: O processo/documento não tem requerimento.
				// Msg: O requerimento {0} não está mais associado ao processo/documento {1}.
				Validacao.Add(Mensagem.Especificidade.RequerimentoNaoAssociadoProcesso(especificidade.ProtocoloReq.RequerimentoId, especificidade.ProtocoloReq.Numero));
				return false;
			}

			_da.ObterAtividadesNome(especificidade.Atividades);

			if (especificidade.Atividades != null)
			{
				foreach (var item in especificidade.Atividades)
				{
					if(!item.Ativada)
					{
						Validacao.Add(Mensagem.AtividadeEspecificidade.AtividadeDesativada(item.NomeAtividade));
					}
				}
			}

			if (apenasObrigatoriedade)
			{
				return Validacao.EhValido;
			}

			if (faseAnterior)
			{
				// Msg: O título anterior deve estar encerrado.	
				foreach (var atividadeEsp in especificidade.Atividades)
				{
					if (!TituloAnteriorEncerrado(especificidade.ProtocoloReq.Id, atividadeEsp.Id))
					{
						return false;
					}
				}
			}

			bool ehValido = true;

			if (especificidade.Atividades != null)
			{
				object retorno = false;

				foreach (var item in especificidade.Atividades)
				{
					if (jaAssociado)
					{
						//Validação da existencia de um outro título do mesmo modelo e atividade
						// Msg: A atividade {2} não pode ser selecionada, pois já está associada ao título {0} - {1}.
						// Msg: A atividade {1} não pode ser selecionada, pois já está associada a outro título \"{0}\".
						if (!ValidarAtividadeJaAssociada(especificidade.ProtocoloReq.Id, especificidade.ProtocoloReq.IsProcesso, item.Id, especificidade.Titulo.Id, Convert.ToInt32(especificidade.Titulo.Modelo)))
						{
							ehValido = false;
							continue;
						}
					}

					retorno = _da.ValidarAtividade(item.Id, especificidade.ProtocoloReq.Id, 1);//Em andamento

					//Validação da situação da atividade e se o protocolo ainda possui a atividade
					if (Convert.GetTypeCode(retorno) == TypeCode.Boolean)
					{
						if (atividadeAndamento && !Convert.ToBoolean(retorno))
						{
							ehValido = false;
							// Msg: A atividade \"{0}\" deve estar na situação \"em andamento\".
							Validacao.Add(Mensagem.Especificidade.AtividadeSituacaoInvalida(item.NomeAtividade));
						}
					}
					else
					{
						ehValido = false;
						// Msg: A atividade \"{0}\" não está mais associada ao {2} {1}. Favor salvar as alterações no título.
						Validacao.Add(Mensagem.Especificidade.AtividadeNaoAssociadaRequerimento(item.NomeAtividade, especificidade.ProtocoloReq.Numero, especificidade.ProtocoloReq.IsProcesso));
						continue;
					}

					if (solicitado)
					{
						//Validação da solicitação do modelo/atividade no protocolo
						// Msg: Este modelo de título não foi solicitado para a atividade \"{0}\".
						retorno = ValidarModeloNaoRequisitado(especificidade.ProtocoloReq.Id, especificidade.ProtocoloReq.IsProcesso, item.Id, Convert.ToInt32(especificidade.Titulo.Modelo));

						if (!Convert.ToBoolean(retorno))
						{
							ehValido = false;
						}
					}
				}
			}

			return ehValido;
		}

		public bool DeclaratorioRequerimentoAtividade(IEspecificidade especificidade, bool solicitado = true, bool jaAssociado = true, bool apenasObrigatoriedade = false)
		{
			if (especificidade.RequerimentoId <= 0)
			{
				// Msg: O requerimento padrão é obrigatório.
				Validacao.Add(Mensagem.Especificidade.RequerimentoPradroObrigatoria);
			}

			if (especificidade.Atividades == null || especificidade.Atividades.Count == 0 || especificidade.Atividades[0].Id == 0)
			{
				// Msg: A atividade é obrigatória.
				Validacao.Add(Mensagem.Especificidade.AtividadeObrigatoria);
			}

			//Retorna se já encontrou erro de obrigatóriedade
			if (Validacao.Erros.Exists(x => x.Texto == Mensagem.Especificidade.RequerimentoPradroObrigatoria.Texto) || Validacao.Erros.Exists(x => x.Texto == Mensagem.Especificidade.AtividadeObrigatoria.Texto))
			{
				return false;
			}

			_da.ObterAtividadesNome(especificidade.Atividades);

			if (!_da.RequerimentoPossuiEmpreendimento(especificidade.RequerimentoId))
			{
				Validacao.Add(Mensagem.Especificidade.RequerimentoEmpreendimentoObrigatorio(especificidade.Atividades.FirstOrDefault().NomeAtividade));
			}
			else
			{
				if(!_da.EmpreendimentoPossuiCaracterizacaoBarragemDis(especificidade.RequerimentoId))
				{
					Validacao.Add(Mensagem.Especificidade.CaracterizacaoBarragemDisNaoCadastrada);
				}
			}

			if (especificidade.Atividades != null)
			{
				foreach (var item in especificidade.Atividades)
				{
					if (!item.Ativada)
					{
						Validacao.Add(Mensagem.AtividadeEspecificidade.AtividadeDesativada(item.NomeAtividade));
					}
				}
			}

			if (apenasObrigatoriedade)
			{
				return Validacao.EhValido;
			}

			bool ehValido = true;

			if (especificidade.Atividades != null)
			{
				object retorno = false;

				foreach (var item in especificidade.Atividades)
				{
					if (jaAssociado)
					{
						//Validação da existencia de um outro título do mesmo modelo e atividade para o mesmo empreendimento
						if (!DeclaratorioValidarAtividadeJaAssociada(especificidade.RequerimentoId, item.Id, especificidade.Titulo.Id, Convert.ToInt32(especificidade.Titulo.Modelo)))
						{
							ehValido = false;
							continue;
						}
					}

					if (solicitado)
					{
						//Validação da solicitação do modelo/atividade no requerimento
						retorno = DeclaratorioValidarModeloNaoRequisitado(especificidade.RequerimentoId, item.Id, Convert.ToInt32(especificidade.Titulo.Modelo));

						if (!Convert.ToBoolean(retorno))
						{
							ehValido = false;
						}
					}
				}
			}

			return ehValido;
		}

		public bool PossuiAtividadeEmAndamento(int id)
		{
			EspecificidadeDa _da = new EspecificidadeDa();

			if (!_da.ValidarPossuiAtividadeEmAndamento(id))
			{
				Validacao.Add(Mensagem.Especificidade.AtividadesSituacaoAndamento);
				return false;
			}

			return true;
		}

		public bool ValidarAtividadeJaAssociada(int protocolo, bool isProcesso, int atividade, int titulo, int modelo)
		{
			List<TituloEsp> lista = _da.ObterTitulosAtividadeProtocolo(protocolo, isProcesso, atividade, titulo, modelo);

			if (lista != null && lista.Count > 0)
			{
				foreach (TituloEsp tit in lista)
				{
					Validacao.Add(Mensagem.Especificidade.AtividadeEstaAssociadaAOutroTitulo(tit.Numero.Texto, tit.Modelo, _da.ObterAtividadeNome(atividade)));
				}

				return false;
			}

			return true;
		}

		public bool DeclaratorioValidarAtividadeJaAssociada(int requerimento, int atividade, int titulo, int modelo)
		{
			List<TituloEsp> lista = _da.ObterTitulosAtividadeEmpreendimento(requerimento, atividade, titulo, modelo);

			if (lista != null && lista.Count > 0)
			{
				foreach (TituloEsp tit in lista)
				{
					Validacao.Add(Mensagem.Especificidade.AtividadeEstaAssociadaAOutroTitulo(tit.Numero.Texto, tit.Modelo, _da.ObterAtividadeNome(atividade)));
				}

				return false;
			}

			return true;
		}

		public bool ValidarModeloNaoRequisitado(int protocolo, bool isProcesso, int atividade, int modelo)
		{
			if (!_da.ValidarProtocoloAtividadePossuiModelo(protocolo, atividade, modelo))
			{
				Validacao.Add(Mensagem.Especificidade.AtividadeComOutroModeloTitulo(_da.ObterAtividadeNome(atividade)));
				return false;
			}

			return true;
		}

		public bool DeclaratorioValidarModeloNaoRequisitado(int requerimento, int atividade, int modelo)
		{
			if (!_da.ValidarRequerimentoAtividadePossuiModelo(requerimento, atividade, modelo))
			{
				Validacao.Add(Mensagem.Especificidade.AtividadeComOutroModeloTitulo(_da.ObterAtividadeNome(atividade)));
				return false;
			}

			return true;
		}

		public bool TituloAnteriorEncerrado(int processoDocumentoId, int atividadeId)
		{
			List<TituloAnterior> titulos = new List<TituloAnterior>();

			if (!_da.ValidarTituloAnteriorEncerrado(processoDocumentoId, atividadeId, out titulos))
			{
				foreach (var item in titulos)
				{
					Validacao.Add(Mensagem.Especificidade.TituloAnteriorNaoEncerrado(item.Atividade, item.Modelo));
				}

				return false;
			}
			return true;
		}

		public void Destinatario(int protocolo, List<DestinatarioEspecificidade> destinatarios, string campo = "ddlDestinatarioEsp")
		{
			if (destinatarios == null || destinatarios.Count == 0 || destinatarios.Any(x => x.Id == 0))
			{
				Validacao.Add(Mensagem.Especificidade.DestinatarioObrigatorio(campo));
				return;
			}

			List<PessoaLst> lstDestinatarios = _da.ObterInteressados(protocolo);

			destinatarios.ForEach(x =>
			{

				if (!lstDestinatarios.Any(des => des.Id == x.Id))
				{
					Validacao.Add(Mensagem.Especificidade.DestinatarioDesassociado(campo, x.Nome));
				}
			});
		}

		public bool Destinatario(int protocolo, int destinatario, string campo)
		{
			if (destinatario <= 0)
			{
				Validacao.Add(Mensagem.Especificidade.DestinatarioObrigatorio(campo));
				return false;
			}

			if ((_da.ObterInteressados(protocolo)).Count(x => x.Id == destinatario) == 0)
			{
				Validacao.Add(Mensagem.Especificidade.DestinatarioDesassociado(campo));
				return false;
			}

			return true;
		}

		public bool ValidarTituloGenericoAtividadeCaracterizacao(IEspecificidade especificidade, eEspecificidade tipo = eEspecificidade.Nulo)
		{
			CaracterizacaoBus busCaracterizacao = new CaracterizacaoBus();
			GerenciadorConfiguracao configAtividade = new GerenciadorConfiguracao(new ConfiguracaoAtividade());
			GerenciadorConfiguracao configCaracterizacao = new GerenciadorConfiguracao(new ConfiguracaoCaracterizacao());
			List<AtividadeCaracterizacao> lstAtivCaract = configAtividade.Obter<List<AtividadeCaracterizacao>>(ConfiguracaoAtividade.KeyAtividadesCaracterizacoes);
			List<Caracterizacao> lstCaracterizacoes = busCaracterizacao.ObterCaracterizacoesEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault());
			List<CaracterizacaoLst> lstCnfCaracterizacoes = configCaracterizacao.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
			//List<DependenciaLst> lstCnfDependencias = configCaracterizacao.Obter<List<DependenciaLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoesDependencias);
			EspecificidadeBusBase _busEspBase = new EspecificidadeBusBase();

			foreach (var item in especificidade.Atividades)
			{
				if (item.Id == 0)
				{
					continue;
				}

				var ativCaract = lstAtivCaract.FirstOrDefault(x => x.AtividadeId == item.Id);

				if (ativCaract == null || ativCaract.AtividadeId != item.Id || !ativCaract.IsTituloGenerico)
				{
					EspecificidadeDa _daEsp = new EspecificidadeDa();
					item.NomeAtividade = _daEsp.ObterAtividadeNome(item.Id);
					Validacao.Add(Mensagem.Especificidade.AtividadeNaoConfiguradaNaAtividadeCaracte(item.NomeAtividade, especificidade.Titulo.ModeloSigla));
					continue;
				}

				Caracterizacao caracterizacao = lstCaracterizacoes.Find(x => x.Tipo == (eCaracterizacao)ativCaract.CaracterizacaoTipoId);
				CaracterizacaoLst caracterizacaoTipo = lstCnfCaracterizacoes.Find(x => x.Id == ativCaract.CaracterizacaoTipoId);
				ICaracterizacaoBus busCaract = CaracterizacaoBusFactory.Criar(caracterizacao.Tipo);

				if (busCaract == null)
				{
					throw new Exception("CaracterizacaoBusFactory não implementada para o tipo " + caracterizacaoTipo.Texto);
				}

				List<int> atividadesCaract = busCaract.ObterAtividadesCaracterizacao(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault());

				if (atividadesCaract == null || atividadesCaract.Count == 0)
				{
					Validacao.Add(Mensagem.Especificidade.CaracterizacaoNaoPreenchida(caracterizacaoTipo.Texto));
					continue;
				}

				if (!atividadesCaract.Exists(x => x == item.Id))
				{
					Validacao.Add(Mensagem.Especificidade.AtividadeDiferenteCaracterizacao(caracterizacaoTipo.Texto));
					continue;
				}

				List<Dependencia> lstDependenciaAteradas = busCaracterizacao.ObterDependenciasAlteradas(
					especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(),
					caracterizacao.Id,
					(eCaracterizacao)caracterizacaoTipo.Id,
					eCaracterizacaoDependenciaTipo.Caracterizacao);

				if (lstDependenciaAteradas != null && lstDependenciaAteradas.Count > 0)
				{
					Validacao.Add(Mensagem.Especificidade.CaracterizacaoDependencias(caracterizacaoTipo.Texto));
					/*Validacao.Add(Mensagem.Especificidade.CaracterizacaoDependencias(String.Join(", ", lstDependenciaAteradas.Select(x =>
						String.Format("{0} - {1}", 
						lstCnfDependencias.Single(y => y.Id == x.DependenciaTipo).TipoTexto, 
						lstCnfCaracterizacoes.Single(y => y.Id == x.DependenciaCaracterizacao).Texto)
					).ToArray())));*/
				}
			}

			switch (tipo)
			{
				case eEspecificidade.LicencaAmbientalRegularizacao:
				case eEspecificidade.LicencaAmbientalUnica:
				case eEspecificidade.LicencaSimplificada:
				case eEspecificidade.LicencaInstalacao:
				case eEspecificidade.LicencaOperacao:
				case eEspecificidade.LicencaPrevia:
					if (especificidade.Atividades.Count > 0)
					{
						especificidade.AtividadeCaractTipo =
							(eAtividadeCodigo) ConfiguracaoAtividade.ObterCodigo(especificidade.Atividades[0].Id);
					}

					switch (especificidade.AtividadeCaractTipo)
					{
						case eAtividadeCodigo.Barragem:
							var configEspAtivCaract = _busEspBase.GetConfigAtivEspCaracterizacao((int)tipo);
							var list = configEspAtivCaract.GetListValue<int>("Atividades");

							if (especificidade is ILicenca && list.Where(x => x == (int)eAtividadeCodigo.Barragem).ToList().Count > 0)
							{
								_validarLicenca.Validar(especificidade as ILicenca);
							}

							break;

					}
					break;
			}
			return Validacao.EhValido;
		}

		public bool ExisteProcDocFilhoQueFoiDesassociado(int titulo, BancoDeDados banco = null)
		{
			try
			{
				return _da.ExisteProcDocFilhoQueFoiDesassociado(titulo, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return false;
		}

		public bool ValidarDestinatarioIsRepresentanteEmpreendimento(int protocolo, int destinatario, BancoDeDados banco = null)
		{
			try
			{
				return _da.ValidarDestinatarioIsRepresentanteEmpreendimento(protocolo, destinatario, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return false;
		}
	}
}