using System;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAgrotoxico.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAgrotoxico.Business
{
	class AgrotoxicoValidar
	{
		#region Propriedades

		AgrotoxicoDa _da = new AgrotoxicoDa();
		PessoaDa _pessoaDa = new PessoaDa();

		#endregion

		internal bool Salvar(Agrotoxico agrotoxico)
		{
			if (agrotoxico.Id > 0)
			{
				if (!_da.Existe(agrotoxico.Id))
				{
					Validacao.Add(Mensagem.Agrotoxico.AgrotoxicoInexistente);
					return Validacao.EhValido;
				}
			}

			if (agrotoxico.PossuiCadastro)
			{
				if (agrotoxico.NumeroCadastro < 1)
				{
					Validacao.Add(Mensagem.Agrotoxico.NumeroCadastroObrigatorio);
				}
				else
				{
					GerenciadorConfiguracao<ConfiguracaoSistema> configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
					if (agrotoxico.NumeroCadastro > Convert.ToInt32(configSys.Obter<String>(ConfiguracaoSistema.KeyAgrotoxicoMaxNumeroCadastro)))
					{
						Validacao.Add(Mensagem.Agrotoxico.NumeroCadastroSuperiorMaximo);
					}
					else if (_da.NumeroCadastroExiste(agrotoxico))
					{
						Validacao.Add(Mensagem.Agrotoxico.NumeroCadastroExistente);
					}
				}
			}

			if (string.IsNullOrEmpty(agrotoxico.NomeComercial))
			{
				Validacao.Add(Mensagem.Agrotoxico.NomeComercialObrigatorio);
			}

			if (agrotoxico.NumeroRegistroMinisterio <= 0)
			{
				Validacao.Add(Mensagem.Agrotoxico.NumeroRegistroObrigatorio);
			}

			if (agrotoxico.NumeroProcessoSep <= 0)
			{
				Validacao.Add(Mensagem.Agrotoxico.NumeroProcessoSepObrigatorio);
			}

			if (agrotoxico.TitularRegistro.Id <= 0)
			{
				Validacao.Add(Mensagem.Agrotoxico.TitularRegistroObrigatorio);
			}
			else
			{
				if (!_pessoaDa.ExistePessoa(agrotoxico.TitularRegistro.Id))
				{
					Validacao.Add(Mensagem.Agrotoxico.PessoaNaoCadastrada(agrotoxico.TitularRegistro.NomeRazaoSocial));
				}
			}

			if (agrotoxico.PericulosidadeAmbiental.Id <= 0)
			{
				Validacao.Add(Mensagem.Agrotoxico.PericulosidadeAmbientalObrigatorio);
			}

			if (agrotoxico.ClassificacaoToxicologica.Id <= 0)
			{
				Validacao.Add(Mensagem.Agrotoxico.ClassificacaoToxicologicaObrigatorio);
			}

			if (agrotoxico.FormaApresentacao.Id <= 0)
			{
				Validacao.Add(Mensagem.Agrotoxico.FormaApresentacaoObrigatorio);
			}

			if (_da.ProcessoSepEmOutroCadastro(agrotoxico))
			{
				Validacao.Add(Mensagem.Agrotoxico.ProcessoSepEmOutroCadastro);
			}

			if (agrotoxico.ClassesUso.Count < 1)
			{
				Validacao.Add(Mensagem.Agrotoxico.ClasseUsoObrigatorio);
			}

			if (agrotoxico.GruposQuimicos.Count < 1)
			{
				Validacao.Add(Mensagem.Agrotoxico.GrupoQuimicoObrigatorio);
			}

			if (agrotoxico.IngredientesAtivos.Count < 1)
			{
				Validacao.Add(Mensagem.Agrotoxico.IngredienteAtivoObrigatorio);
			}
			else
			{
				IngredienteAtivoBus ingredienteAtivoBus = new IngredienteAtivoBus();
				ingredienteAtivoBus.ObterValores(agrotoxico.IngredientesAtivos);

				agrotoxico.IngredientesAtivos.ForEach(ingrediente =>
				{
					//if (ingrediente.Concentracao <= 0)
					//{
					//	Validacao.Add(Mensagem.Agrotoxico.ConcentracaoObrigatorio);
					//}

					//if (ingrediente.UnidadeMedidaId <= 0)
					//{
					//	Validacao.Add(Mensagem.Agrotoxico.UnidadeMedidaObrigatoria);
					//}

					//if (ingrediente.UnidadeMedidaId == (int)eAgrotoxicoIngredienteAtivoUnidadeMedida.Outros && string.IsNullOrEmpty(ingrediente.UnidadeMedidaOutro))
					//{
					//	Validacao.Add(Mensagem.Agrotoxico.UnidadeMedidaOutroObrigatorio);
					//}

					if (ingrediente.SituacaoId == (int)eIngredienteAtivoSituacao.Inativo)
					{
						Validacao.Add(Mensagem.Agrotoxico.IngredienteAtivoInativo(ingrediente.Texto));
					}
				});
			}

			return Validacao.EhValido;
		}

		internal bool Excluir(int id)
		{
			String tituloSituacao = _da.PossuiTituloAssociado(id);
			if (!String.IsNullOrWhiteSpace(tituloSituacao))
			{
				Validacao.Add(Mensagem.Agrotoxico.PossuiTituloAssociado(tituloSituacao));
			}

			return Validacao.EhValido;
		}
	}
}