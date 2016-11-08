using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business
{
	public class EntregaBus
	{
		#region Propriedades
		
		EntregaValidar _validar = null;
		EntregaDa _da = new EntregaDa();
		TituloDa _daTitulo = new TituloDa();
		TituloBus _busTitulo = new TituloBus();
		TituloModeloBus _busModelo = new TituloModeloBus();
		TituloSituacaoBus _busTituloSituacao = new TituloSituacaoBus();
		PessoaBus _busPessoa = new PessoaBus();

		#endregion

		public EntregaBus(EntregaValidar entregaValidar)
		{
			_validar = entregaValidar;
		}

		public void Salvar(Entrega entrega)
		{	
			try
			{
				if (_validar.Salvar(entrega))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(entrega, bancoDeDados);

						foreach (int item in entrega.Titulos)
						{
							Titulo titulo = _busTitulo.Obter(item);
							titulo.Modelo = _busModelo.Obter(titulo.Modelo.Id);

							if (titulo.Situacao.Id == 4)
							{
								_busTituloSituacao.AlterarSituacao(titulo, 7, bancoDeDados);

								if (!Validacao.EhValido)
								{
									return;
								}
							}
						}

						bancoDeDados.Commit();
						Validacao.Add(Mensagem.Entrega.Cadastrar);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public Resultados<Entrega> Filtrar(ListarEntregaFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ListarEntregaFiltro> filtro = new Filtro<ListarEntregaFiltro>(filtrosListar, paginacao);
				return _validar.Filtrar(_da.Filtrar(filtro));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Pessoa ObterPessoa(string cpf)
		{
			try
			{
				if (_validar.ValidarCPF(cpf))
				{
					Pessoa pessoa = _busPessoa.Obter(cpf);

					if(pessoa.Id == 0)
					{
						Validacao.Add(Mensagem.Entrega.PessoaNaoCadastrada);
					}

					return pessoa;
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Entrega ObterTituloEntrega(Protocolo protocolo)
		{
			Entrega entrega = new Entrega();

			try
			{
				entrega.Protocolo = new Protocolo( _validar.ProcessoEntregaTitulo(protocolo.Numero));

				if (!Validacao.EhValido)
				{
					return entrega;
				}

				List<Titulo> titulos = null;

				titulos = _daTitulo.TitulosProtocolo(entrega.Protocolo.Id.Value);
				
				if (titulos == null)
				{
					Validacao.Add(Mensagem.Entrega.NenhumTituloEncontrado);
					return entrega;
				}

				foreach (Titulo titulo in titulos)
				{
					titulo.Modelo = _busModelo.Obter(titulo.Modelo.Id);

					if (!_validar.TituloEntrega(titulo))
					{
						continue;
					}
					entrega.TitulosEntrega.Add(titulo);
				}

				if (entrega.TitulosEntrega.Count == 0)
				{
					Validacao.Add(Mensagem.Entrega.NenhumTituloEncontrado);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entrega;
		}

		public string ObterTitulosConcluirEntrega(Protocolo protocolo, List<Int32> titulosSelecionados)
		{
			_validar.ValidarProtocoloNumero(protocolo.Numero);
			
			if (titulosSelecionados == null || titulosSelecionados.Count <= 0)
			{
				Validacao.Add(Mensagem.Entrega.TituloObrigatorio);
				return string.Empty;
			}

			string numeros = string.Empty;

			try
			{
				List<Titulo> titulos = null;

				titulos = _daTitulo.TitulosProtocolo(protocolo.Id.Value);
				
				List<String> numTitulosAssinados = new List<String>();

				foreach (Titulo titulo in titulos)
				{
					titulo.Modelo = _busModelo.Obter(titulo.Modelo.Id);

					TituloModeloResposta resposta = titulo.Modelo.Resposta(eRegra.Prazo, eResposta.InicioPrazo);
					if (resposta != null && resposta.Valor.ToString() == "3")//início do prazo da data de entrega
					{
						//ver id da Situacao "Assinado" na lov_titulo_situacao 
						if (titulo.Situacao.Id == 4 && titulosSelecionados.Exists(x => x == titulo.Id))
						{
							numTitulosAssinados.Add(titulo.Numero.Texto + " - " + titulo.Modelo.Nome);
						}
					}
				}

				if (numTitulosAssinados != null && numTitulosAssinados.Count > 0)
				{
					numeros = Mensagem.Concatenar(numTitulosAssinados);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return numeros;
		}
	}
}