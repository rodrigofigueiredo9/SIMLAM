using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBeneficiamentoMadeira;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBeneficiamentoMadeira.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBeneficiamentoMadeira.Business
{
	public class BeneficiamentoMadeiraValidar
	{
		#region Propriedades

		BeneficiamentoMadeiraDa _da = new BeneficiamentoMadeiraDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CoordenadaAtividadeValidar _coordenadaValidar = new CoordenadaAtividadeValidar();

		#endregion

		internal bool Salvar(BeneficiamentoMadeira caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new BeneficiamentoMadeira()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			List<String> atividadesDuplicadas = new List<String>();

			foreach (var item in caracterizacao.Beneficiamentos)
			{

				#region Atividade

				if (item.Atividade <= 0)
				{
					Validacao.Add(Mensagem.BeneficiamentoMadeira.AtividadeObrigatoria(item.Identificador));
				}
				else
				{

					if (caracterizacao.Beneficiamentos.Where(x => x.Atividade == item.Atividade).ToList().Count >= 2) {
						atividadesDuplicadas.Add(item.Identificador);
					}

					if (item.Atividade == ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SerrariasQuandoNaoAssociadasAFabricacaoDeEstruturas))
					{
						if (!String.IsNullOrWhiteSpace(item.VolumeMadeiraSerrar))
						{
							decimal aux = 0;

							if (Decimal.TryParse(item.VolumeMadeiraSerrar, out aux))
							{
								if (aux <= 0)
								{
									Validacao.Add(Mensagem.BeneficiamentoMadeira.VolumeMadeiraSerrarMaiorZero);
								}
							}
							else
							{
								Validacao.Add(Mensagem.BeneficiamentoMadeira.VolumeMadeiraSerrarInvalido);
							}
						}
						else
						{
							Validacao.Add(Mensagem.BeneficiamentoMadeira.VolumeMadeiraSerrarObrigatorio);
						}
					}


					if (item.Atividade == ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.FabricacaoDeEstruturasDeMadeiraComAplicacaoRural))
					{
						if (!String.IsNullOrWhiteSpace(item.VolumeMadeiraProcessar))
						{
							decimal aux = 0;

							if (Decimal.TryParse(item.VolumeMadeiraProcessar, out aux))
							{
								if (aux <= 0)
								{
									Validacao.Add(Mensagem.BeneficiamentoMadeira.VolumeMadeiraProcessarMaiorZero);
								}
							}
							else
							{
								Validacao.Add(Mensagem.BeneficiamentoMadeira.VolumeMadeiraProcessarInvalido);
							}
						}
						else
						{
							Validacao.Add(Mensagem.BeneficiamentoMadeira.VolumeMadeiraProcessarObrigatorio);
						}
					}
				}



				#endregion

				#region Coordenadas 

				if (item.CoordenadaAtividade.Id <= 0)
				{
					Validacao.Add(Mensagem.BeneficiamentoMadeira.CoordenadaAtividadeObrigatoria(item.Identificador));
				}

				if (item.CoordenadaAtividade.Tipo <= 0)
				{
					Validacao.Add(Mensagem.BeneficiamentoMadeira.GeometriaTipoObrigatorio(item.Identificador));
				}

				#endregion

				#region Materias

				if (item.MateriasPrimasFlorestais.Count <= 0)
				{
					Validacao.Add(Mensagem.MateriaPrimaFlorestalConsumida.MateriaObrigatoria);
				}

				#endregion

				if (String.IsNullOrWhiteSpace(item.EquipControlePoluicaoSonora))
				{
					Validacao.Add(Mensagem.BeneficiamentoMadeira.EquipControlePoluicaoSonoraObrigatorio(item.Identificador));
				}
			}

			if (atividadesDuplicadas.Count > 0)
			{
				Validacao.Add(Mensagem.BeneficiamentoMadeira.AtividadeDuplicada(atividadesDuplicadas));
			}


			return Validacao.EhValido;

		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.BeneficiamentoMadeira))
			{
				return false;
			}

			return Validacao.EhValido;
		}
	}
}
