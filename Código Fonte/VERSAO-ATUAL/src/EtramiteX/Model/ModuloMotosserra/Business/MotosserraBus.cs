using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloMotosserra;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloMotosserra.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloMotosserra.Business
{
	public class MotosserraBus
	{
		MotosserraDa _da = new MotosserraDa();
		MotosserraValidar _validar = new MotosserraValidar();

		#region Comandos DML's

		public bool Salvar(Motosserra motosserra)
		{
			try
			{
				if (_validar.Salvar(motosserra))
				{
					Motosserra aux = Obter(motosserra.Id);

					if (!motosserra.PossuiRegistro)
					{
						if (!aux.PossuiRegistro)
						{
							motosserra.RegistroNumero = aux.RegistroNumero;
						}
						else
						{
							motosserra.RegistroNumero = 0;
						}
					}

					GerenciadorTransacao.ObterIDAtual();
					Mensagem mensagem = (motosserra.Id > 0) ? Mensagem.Motosserra.Editar : Mensagem.Motosserra.Salvar;

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(motosserra, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(mensagem);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool AlterarSituacao(Motosserra motosserra)
		{
			try
			{
				if (_validar.AlterarSituacao(motosserra))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.AlterarSituacao(motosserra, bancoDeDados);

						Validacao.Add(Mensagem.Motosserra.AlterarSituacao);

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

		public void Excluir(int id)
		{
			if (_validar.Excluir(id))
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(id, bancoDeDados);

					Validacao.Add(Mensagem.Motosserra.Excluir);

					bancoDeDados.Commit();
				}
			}
		}

		#endregion Comandos DML's

		#region Obter/Filtrar

		public Motosserra Obter(int id)
		{
			try
			{
				return _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<Motosserra> Filtrar(MotosserraListarFiltros filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<MotosserraListarFiltros> filtro = new Filtro<MotosserraListarFiltros>(filtrosListar, paginacao);
				Resultados<Motosserra> resultados = _da.Filtrar(filtro);

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

		#endregion Obter/Filtrar

		#region Validacoes

		public List<Motosserra> Verificar(String numero, out bool podeCriarNovo)
		{
			podeCriarNovo = false;
			List<Motosserra> motosserras = new List<Motosserra>();

			try
			{
				if (_validar.Verificar(numero))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						if (!_da.ExisteNumero(numero))
						{
							podeCriarNovo = true;
						}

						motosserras = _da.ObterPorNumero(numero);

						if (motosserras.Count == 0)
						{
							Validacao.Add(Mensagem.Motosserra.SerieNumeroDisponivel);
						}

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return motosserras;

		}

		#endregion
	}
}