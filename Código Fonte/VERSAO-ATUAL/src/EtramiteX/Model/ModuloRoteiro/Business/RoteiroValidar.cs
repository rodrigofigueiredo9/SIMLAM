using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Business
{
	public class RoteiroValidar
	{
		RoteiroDa _da = new RoteiroDa();
		ArquivoValidar _validarArq = new ArquivoValidar();
		AtividadeConfiguracaoBus _atividadeBus = new AtividadeConfiguracaoBus();
		ListaBus _busLista = new ListaBus();
		FuncionarioBus _busFuncionario = new FuncionarioBus();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		private List<Roteiro> RoteiroPadrao
		{
			get { return _busLista.RoteiroPadrao; }
		}

		public bool Salvar(Roteiro roteiro)
		{
			if (roteiro.Id > 0)
			{
				VerificarRoteiroEditar(roteiro);
			}
			else
			{
				VerificarRoteiroCriar(roteiro);
			}
			return Validacao.EhValido;
		}
		
		public bool AbrirEditar(Roteiro roteiro)
		{
			if (roteiro.Situacao != 1)
			{
				Validacao.Add(Mensagem.Roteiro.SituacaoInvalida);
			}

			VerificarEditarSetor(roteiro);

			return Validacao.EhValido;
		}

		private void VerificarRoteiroCriar(Roteiro roteiro)
		{
			if (roteiro.Setor < 1)
			{
				Validacao.Add(Mensagem.Roteiro.ObrigatorioSetor);
			}

			if (String.IsNullOrWhiteSpace(roteiro.Nome))
			{
				Validacao.Add(Mensagem.Roteiro.ObrigatorioNome);
			}

			if (!_busFuncionario.ObterSetoresFuncionario(User.FuncionarioId).Exists(x => x.Id == roteiro.Setor))
			{
				Validacao.Add(Mensagem.Roteiro.FuncionarioSetorDiferente);
			}

			VerificarRoteiroNome(roteiro.Nome, roteiro.Setor, roteiro.Id);

			VerificarTiposArquivos(roteiro.Anexos);

			VerificarAtividade(roteiro);

			VerificarItens(roteiro);

			VerificarRoteiroConfigurado(roteiro);
			
		}

		private void VerificarRoteiroConfigurado(Roteiro roteiro)
		{
			if (roteiro.Atividades.Count < 1 || roteiro.Finalidade == null || roteiro.Modelos.Count < 1)
			{
				return;
			}

			List<string> atividades = new List<string>();
			List<string> modelos = new List<string>();
			List<string> finalidades = new List<string>();

			if (_da.ValidarRoteiroConfigurado(roteiro, out atividades, out modelos, out finalidades))
			{
				Validacao.Add(Mensagem.Roteiro.RoteiroAtividadeConfigurada(atividades, finalidades, modelos));
			}
		}

		private void VerificarItens(Roteiro roteiro)
		{
			if (roteiro.Itens.Count < 1)
			{
				Validacao.Add(Mensagem.Item.ItemObrigatorio);
			}
			else
			{
				foreach (var item in roteiro.Itens)
				{
					if (!_da.ExisteItem(item.Id))
					{
						Validacao.Add(Mensagem.Item.ItemExcluidoSistema(item.Nome));
					}
				}
			}
		}

		private void VerificarAtividade(Roteiro roteiro)
		{
			//Atividade não é obrigatoria
			if (roteiro.Atividades.Count > 0)
			{
				if (roteiro.Finalidade == null)
				{
					Validacao.Add(Mensagem.Roteiro.FinalidadeObrigatorio);
				}

				if (roteiro.Modelos.Count < 1)
				{
					Validacao.Add(Mensagem.Roteiro.ModeloObrigatorio);
				}

				List<TituloModeloLst> lista = _atividadeBus.ObterModelosAtividades(roteiro.Atividades);

				foreach (var modelo in roteiro.Modelos)
				{
					if (!lista.Exists(y => y.Id == modelo.Id))
					{
						Validacao.Add(Mensagem.Roteiro.TituloNaoEncontradoAtividade(modelo.Texto));
					}
				}

				if (!_da.AtividadesEmSetor(roteiro.Atividades.Select(x => x.Id).ToList<int>(), roteiro.Setor))
				{
					Validacao.Add(Mensagem.Roteiro.AtividadeSetorDiferenteRoteiro);
				}

				roteiro.Atividades.ForEach(atividade =>
				{
					if (!_da.IsAtividadeAtiva(atividade.Id))
					{
						Validacao.Add(Mensagem.Roteiro.AtividadeDesativada(atividade.Texto));
					}
				});
			}
		}

		private void VerificarTiposArquivos(List<Anexo> list)
		{
			List<Arquivo> arquivos = new List<Arquivo>();
			List<String> tipos = new List<String>();
			tipos.Add(eTipoArquivo.pdf.ToString());

			if (list != null && list.Count > 0)
			{
				foreach (Anexo anexo in list)
				{
					arquivos.Add(anexo.Arquivo);
				}
			}
			_validarArq.VerificarTiposArquivosValidos(tipos, arquivos);
		}

		private void VerificarRoteiroNome(string nome, int setor, int id)
		{
			List<Roteiro> lista = new List<Roteiro>();
			lista = _da.ObterNomeRoteiro(nome);

			if (lista.Exists(x => x.Setor == setor && x.Id != id))
			{
				Validacao.Add(Mensagem.Roteiro.NomeExistente);
			}
		}

		private void VerificarRoteiroEditar(Roteiro roteiro)
		{
			if (roteiro.Setor <= 0)
			{
				Validacao.Add(Mensagem.Roteiro.ObrigatorioSetor);
			}

			VerificarEditarSetor(roteiro);

			if (String.IsNullOrWhiteSpace(roteiro.Nome))
			{
				Validacao.Add(Mensagem.Roteiro.ObrigatorioNome);
			}

			VerificarRoteiroNomeEditar(roteiro);

			VerificarAtividade(roteiro);

			VerificarItens(roteiro);

			VerificarRoteiroConfigurado(roteiro);

			if (!_da.ValidarSituacao(roteiro.Id, 1))//ativo
			{
				Validacao.Add(Mensagem.Roteiro.SituacaoInvalida);
			}
		}

		private bool VerificarEditarSetor(Roteiro roteiro)
		{
			if (!_busFuncionario.ObterSetoresFuncionario(User.FuncionarioId).Exists(x => x.Id == roteiro.Setor))
			{
				Validacao.Add(Mensagem.Roteiro.NaoEditarRoteiroDeOutroSetor);
			}
			return Validacao.EhValido;
		}

		private void VerificarRoteiroNomeEditar(Roteiro roteiro)
		{
			List<Roteiro> lista = new List<Roteiro>();
			lista = _da.ObterNomeRoteiro(roteiro.Nome);

			if (lista != null && lista.Count > 0)
			{
				foreach (Roteiro item in lista)
				{
					if (item.Numero != roteiro.Numero && item.Setor == roteiro.Setor)
					{
						Validacao.Add(Mensagem.Roteiro.NomeExistente);
					}
				}
			}
		}

		public bool RoteiroIsPadrao(int roteiroId)
		{
			return _busLista.RoteiroPadrao.Exists(x => x.Id == roteiroId);
		}
		
		public bool PossuiModelosAtividades(List<TituloModeloLst> lista, int roteiro = 0)
		{
			if (lista.Count < 1 && !RoteiroIsPadrao(roteiro))
			{
				Validacao.Add(Mensagem.Roteiro.NenhumTituloEncontrado);
			}

			return Validacao.EhValido;
		}

		public bool ValidarDesativarRoteiro(int id)
		{
			if (RoteiroIsPadrao(id))
			{
				Validacao.Add(Mensagem.Roteiro.RoteiroPadrao);
			}

			if (_da.ObterSituacao(id) == 2)
			{
				Validacao.Add(Mensagem.Roteiro.RoteiroDesativo);
			}

			return Validacao.EhValido;
		}

		public bool ValidarRoteiroDesativado(int id)
		{
			if (_da.RoteiroDesativado(id)) {
				Validacao.Add(Mensagem.Roteiro.NaoPodeAssociarDesativado);
			}
			return Validacao.EhValido;
		}

		internal bool ValidarAssociarAtividade(int roteiroSetor, int atividadeId)
		{
			if (!_da.AtividadeEmSetor(atividadeId, roteiroSetor))
			{
				Validacao.Add(Mensagem.Roteiro.AtividadeSetorDiferenteRoteiro);
			}
			return Validacao.EhValido;
		}
	}
}