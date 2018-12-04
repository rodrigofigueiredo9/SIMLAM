using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Tecnomapas.TecnoGeo.Acessadores;
using Tecnomapas.TecnoGeo.Acessadores.OracleSpatial;
using Tecnomapas.TecnoGeo.Geografico;
using Tecnomapas.TecnoGeo.Geometria;
using Tecnomapas.TecnoGeo.Geometria.Agregada;
using Tecnomapas.TecnoGeo.Geometria.Primitiva;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Data
{
	public class LeitorFeicaoGeo : LeitorFeicao
	{
		private ClasseFeicao _tabela;
		private XmlTextReader _dr;

		public string ComandoSQL { get; private set; }

		public LeitorFeicaoGeo(string classeFeicao, FonteFeicao fonte)
		  : this(classeFeicao, fonte, (OperacaoEspacial)null, (Expressao)null)
		{
		}

		public LeitorFeicaoGeo(string classeFeicao, FonteFeicao fonte, OperacaoEspacial operacao, Expressao filtro)
		  : base(classeFeicao, fonte)
		{
			if (fonte == null)
				throw new Exception("Fonte Nula");
			this._fonte = fonte;
			this._tabela = fonte.ObterClasseFeicao(classeFeicao);
			this._atributos = this._tabela.Atributos;
			string str1 = "";
			for (int index = 0; index < this._tabela.Atributos.Count; ++index)
			{
				if (this._tabela.Atributos[index].Tipo == DbType.DateTime)
					str1 = str1 + "to_char(" + this._tabela.Atributos[index].Nome + ",'dd/mm/yyyy HH24:mi:ss') " + this._tabela.Atributos[index].Nome + ", ";
				else
					str1 = str1 + this._tabela.Atributos[index].Nome + ", ";
			}
			if (operacao != null)
				str1 = str1 + operacao.GerarComando("param") + " ";
			string str2 = str1 + this._tabela.CampoGeometrico;
			string str3 = "";
			if (filtro != null)
				str3 = " where " + filtro.GerarComando("fparam");
			OracleCommand cmd1 = new OracleCommand("select count(*) from " + this._tabela.Nome + str3, (this.Fonte as FonteFeicaoOracleSpatial).Conexao);
			filtro?.CarregarParametros(cmd1, "fparam");
			int int32 = Convert.ToInt32(cmd1.ExecuteScalar());
			this._dr = (XmlTextReader)null;
			if (int32 <= 0)
				return;
			OracleCommand cmd2 = new OracleCommand("select " + str2 + " from " + this._tabela.Nome + str3, (this.Fonte as FonteFeicaoOracleSpatial).Conexao);
			cmd2.XmlCommandType = OracleXmlCommandType.Query;
			cmd2.FetchSize = 5242880L;
			cmd2.CommandTimeout = 3600;
			operacao?.CarregarParametros(cmd2, "param");
			filtro?.CarregarParametros(cmd2, "fparam");
			if (cmd2.Parameters.Count > 0)
			{
				cmd2.BindByName = true;
				cmd2.AddRowid = true;
			}
			else
				cmd2.BindByName = false;
			this.ComandoSQL = cmd2.CommandText;
			this._dr = (XmlTextReader)cmd2.ExecuteXmlReader();
			this._dr.WhitespaceHandling = WhitespaceHandling.None;
			do
				;
			while (this._dr.Read() && this._dr.Name != "ROWSET");
		}

		public override void Fechar()
		{
			this.Fonte.Fechar();
		}

		public override bool Ler()
		{
			string str = "";
			Feicao feicao = this._tabela.CriarFeicao();
			try
			{
				if (this._dr != null && this._dr.Read() && this._dr.Name == "ROW" && this._dr.NodeType == XmlNodeType.Element)
				{
					this._atual = this._tabela.CriarFeicao();
					while (this._dr.Read() && this._dr.Name != "ROW" && this._dr.NodeType != XmlNodeType.EndElement)
					{
						string name = this._dr.Name;
						if (this._tabela.CampoGeometrico == name)
						{
							bool tem_srid = false;
							int srid = 0;
							bool tem_ponto = false;
							Decimal pontox = new Decimal(0);
							Decimal pontoy = new Decimal(0);
							Decimal pontoz = new Decimal(0);
							ArrayList arrayList1 = new ArrayList();
							ArrayList arrayList2 = new ArrayList();
							this._dr.Read();
							if (this._dr.Name == "SDO_GTYPE" && this._dr.NodeType == XmlNodeType.Element)
							{
								this._dr.Read();
								int int32 = Convert.ToInt32(this._dr.Value);
								while (this._dr.Read() && this._dr.Name != name)
								{
									if (this._dr.NodeType == XmlNodeType.Element)
									{
										switch (this._dr.Name.ToString())
										{
											case "SDO_SRID":
												this._dr.Read();
												srid = Convert.ToInt32(this._dr.Value);
												tem_srid = true;
												this._dr.Read();
												break;
											case "SDO_POINT":
												while (this._dr.Read() && this._dr.Name != "SDO_POINT")
												{
													if (this._dr.NodeType == XmlNodeType.Element)
													{
														switch (this._dr.Name.ToString())
														{
															case "X":
																this._dr.Read();
																pontox = (Decimal)Convert.ToInt32(this._dr.Value);
																break;
															case "Y":
																this._dr.Read();
																pontoy = (Decimal)Convert.ToInt32(this._dr.Value);
																break;
															case "Z":
																this._dr.Read();
																pontoz = (Decimal)Convert.ToInt32(this._dr.Value);
																break;
														}
														tem_ponto = true;
													}
												}
												break;
											case "SDO_ELEM_INFO":
												while (this._dr.Read() && this._dr.Name != "SDO_ELEM_INFO")
												{
													if (this._dr.Name == "NUMBER" && this._dr.NodeType == XmlNodeType.Element)
													{
														this._dr.Read();
														arrayList1.Add((object)Decimal.Parse(this._dr.Value, NumberStyles.Any));
														this._dr.Read();
													}
												}
												break;
											case "SDO_ORDINATES":
												arrayList2.Add((object)new Decimal(0));
												while (this._dr.Read() && this._dr.Name != "SDO_ORDINATES")
												{
													if (this._dr.Name == "NUMBER" && this._dr.NodeType == XmlNodeType.Element)
													{
														this._dr.Read();
														arrayList2.Add((object)Decimal.Parse(this._dr.Value, NumberStyles.Any));
														this._dr.Read();
													}
												}
												break;
										}
									}
								}
								feicao.Geometria = this.InterpretaGeometria(int32, tem_srid, srid, tem_ponto, pontox, pontoy, pontoz, (Decimal[])arrayList1.ToArray(typeof(Decimal)), (Decimal[])arrayList2.ToArray(typeof(Decimal)));
							}
							else
							{
								feicao.Geometria = (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
								while (this._dr.Read() && this._dr.Name != name)
									;
							}
						}
						else
						{
							switch (feicao.Atributos[this._dr.Name].Tipo)
							{
								case DbType.DateTime:
									feicao.Atributos[this._dr.Name].Valor = (object)DateTime.Parse(this.PegaValor(name));
									break;
								case DbType.Decimal:
									feicao.Atributos[this._dr.Name].Valor = (object)Decimal.Parse(this.PegaValor(name), NumberStyles.Any);
									break;
								case DbType.String:
									feicao.Atributos[this._dr.Name].Valor = (object)Convert.ToString(this.PegaValor(name));
									break;
								default:
									throw new Exception("Tipo de campo não tratado \"" + feicao.Atributos[this._dr.Name].Tipo.ToString() + "\"");
							}
							if (feicao.Atributos[this._dr.Name].Nome.StartsWith("ID"))
								str = feicao.Atributos[this._dr.Name].Valor.ToString();
						}
					}
					this._atual = feicao;
				}
				else
				{
					this._atual = (Feicao)null;
					return false;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Erro em InterpretaGeometria - campo_unico : " + str, ex);
			}
			return true;
		}

		private string PegaValor(string campo)
		{
			string str = "";
			if (!(campo == this._dr.Name))
				throw new Exception("Pegando valor do campo diferente de \"" + campo + "\"");
			while (this._dr.Read() && this._dr.HasValue.ToString() == "True" && this._dr.NodeType.ToString() == "Text")
				str = this._dr.Value;
			return str;
		}

		private Tecnomapas.TecnoGeo.Geometria.Geometria InterpretaGeometria(int gtype, bool tem_srid, int srid, bool tem_ponto, Decimal pontox, Decimal pontoy, Decimal pontoz, Decimal[] elem_info, Decimal[] ordinates)
		{
			int int16_1 = (int)Convert.ToInt16(Math.Floor((Decimal)gtype / new Decimal(1000)));
			int int16_2 = (int)Convert.ToInt16(Math.Floor((Decimal)(gtype - int16_1 * 1000) / new Decimal(100)));
			int num = gtype - int16_1 * 1000 - int16_2 * 100;
			bool ehLrs = int16_2 != 0;
			if (int16_1 != 2 && (int16_1 != 3 || ehLrs) && ((int16_1 != 3 || int16_2 != 3) && (int16_1 != 4 || int16_2 != 3)) && (int16_1 != 4 || int16_2 != 4) || (tem_ponto == elem_info.Length > 0 || elem_info.Length > 0 != ordinates.Length > 0) || (tem_ponto && ehLrs || tem_ponto && num != 1 && num != 4 && num != 5))
				return (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
			switch (num)
			{
				case 0:
					return (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
				case 1:
					if (tem_ponto)
						return (Tecnomapas.TecnoGeo.Geometria.Geometria)new Ponto(int16_1 == 2 ? new Posicao(pontox, pontoy) : new Posicao(pontox, pontoy, pontoz), int16_1, ehLrs);
					PontoCollection pontoCollection1 = this.GerarPontos(elem_info, ordinates, int16_1, int16_2, false);
					if (pontoCollection1 == null || pontoCollection1.Count == 0)
						return (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
					return (Tecnomapas.TecnoGeo.Geometria.Geometria)pontoCollection1[0];
				case 2:
					LinhaCollection linhaCollection1 = this.GerarLinhas(elem_info, ordinates, int16_1, int16_2, false);
					if (linhaCollection1 == null || linhaCollection1.Count == 0)
						return (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
					return (Tecnomapas.TecnoGeo.Geometria.Geometria)linhaCollection1[0];
				case 3:
					PoligonoCollection poligonoCollection1 = this.GerarPoligonos(elem_info, ordinates, int16_1, int16_2, false);
					if (poligonoCollection1 == null || poligonoCollection1.Count == 0)
						return (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
					return (Tecnomapas.TecnoGeo.Geometria.Geometria)poligonoCollection1[0];
				case 4:
					if (tem_ponto)
					{
						Posicao posicao = int16_1 == 2 ? new Posicao(pontox, pontoy) : new Posicao(pontox, pontoy, pontoz);
						Tecnomapas.TecnoGeo.Geometria.Complexa.Complexa complexa = new Tecnomapas.TecnoGeo.Geometria.Complexa.Complexa(int16_1, ehLrs);
						complexa.Elementos.Adicionar((Tecnomapas.TecnoGeo.Geometria.Geometria)new Ponto(posicao, int16_1, ehLrs));
						return (Tecnomapas.TecnoGeo.Geometria.Geometria)complexa;
					}
					PontoCollection pontoCollection2 = this.GerarPontos(elem_info, ordinates, int16_1, int16_2, true);
					if (pontoCollection2 == null)
						return (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
					LinhaCollection linhaCollection2 = this.GerarLinhas(elem_info, ordinates, int16_1, int16_2, true);
					if (linhaCollection2 == null)
						return (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
					PoligonoCollection poligonoCollection2 = this.GerarPoligonos(elem_info, ordinates, int16_1, int16_2, true);
					if (poligonoCollection2 == null)
						return (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
					Tecnomapas.TecnoGeo.Geometria.Complexa.Complexa complexa1 = new Tecnomapas.TecnoGeo.Geometria.Complexa.Complexa(ehLrs ? int16_1 - 1 : int16_1, ehLrs);
					for (int index = 0; index < pontoCollection2.Count; ++index)
						complexa1.Elementos.Adicionar((Tecnomapas.TecnoGeo.Geometria.Geometria)pontoCollection2[index]);
					for (int index = 0; index < linhaCollection2.Count; ++index)
						complexa1.Elementos.Adicionar((Tecnomapas.TecnoGeo.Geometria.Geometria)linhaCollection2[index]);
					for (int index = 0; index < poligonoCollection2.Count; ++index)
						complexa1.Elementos.Adicionar((Tecnomapas.TecnoGeo.Geometria.Geometria)poligonoCollection2[index]);
					if (complexa1.Elementos.Count > 0)
						return (Tecnomapas.TecnoGeo.Geometria.Geometria)complexa1;
					return (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
				case 5:
					if (tem_ponto)
						return (Tecnomapas.TecnoGeo.Geometria.Geometria)new MultiPonto(new Ponto(int16_1 == 2 ? new Posicao(pontox, pontoy) : new Posicao(pontox, pontoy, pontoz), int16_1, ehLrs), int16_1, ehLrs);
					PontoCollection pontos = this.GerarPontos(elem_info, ordinates, int16_1, int16_2, true);
					if (pontos == null || pontos.Count == 0)
						return (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
					return (Tecnomapas.TecnoGeo.Geometria.Geometria)new MultiPonto(pontos, ehLrs ? int16_1 - 1 : int16_1, ehLrs);
				case 6:
					LinhaCollection linhas = this.GerarLinhas(elem_info, ordinates, int16_1, int16_2, true);
					if (linhas == null || linhas.Count == 0)
						return (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
					return (Tecnomapas.TecnoGeo.Geometria.Geometria)new MultiLinha(linhas, ehLrs ? int16_1 - 1 : int16_1, ehLrs);
				case 7:
					PoligonoCollection poligonos = this.GerarPoligonos(elem_info, ordinates, int16_1, int16_2, true);
					if (poligonos == null || poligonos.Count == 0)
						return (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
					return (Tecnomapas.TecnoGeo.Geometria.Geometria)new MultiPoligono(poligonos, ehLrs ? int16_1 - 1 : int16_1, ehLrs);
				default:
					return (Tecnomapas.TecnoGeo.Geometria.Geometria)null;
			}
		}

		private PontoCollection GerarPontos(Decimal[] elem_info, Decimal[] ordinates, int dimensoes, int eixoLrs, bool multiplo)
		{
			bool flag = eixoLrs != 0;
			try
			{
				switch (dimensoes)
				{
					case 2:
						return this.GerarPontos2D(elem_info, ordinates, multiplo);
					case 3:
						if (flag)
							return this.GerarPontosLRS2D(elem_info, ordinates, multiplo);
						return this.GerarPontos3D(elem_info, ordinates, multiplo);
					case 4:
						return this.GerarPontosLRS3D(elem_info, ordinates, eixoLrs, multiplo);
					default:
						return (PontoCollection)null;
				}
			}
			catch
			{
				return (PontoCollection)null;
			}
		}

		private PontoCollection GerarPontos2D(Decimal[] elem_info, Decimal[] ordinates, bool multiplo)
		{
			PontoCollection pontoCollection = new PontoCollection();
			int index1 = 0;
			while (index1 < elem_info.Length)
			{
				if (elem_info[index1 + 1] == new Decimal(1))
				{
					try
					{
						int int32 = Convert.ToInt32(elem_info[index1]);
						if (multiplo)
						{
							int num = index1 < elem_info.Length - 3 ? Convert.ToInt32((elem_info[index1 + 3]) - 1) : ordinates.Length - 1;
							int index2 = int32;
							while (index2 < num)
							{
								Posicao posicao = new Posicao(ordinates[index2], ordinates[index2 + 1], false);
								pontoCollection.Adicionar(new Ponto(posicao, 2, false));
								index2 += 2;
							}
						}
						else
						{
							Posicao posicao = new Posicao(ordinates[int32], ordinates[int32 + 1], false);
							pontoCollection.Adicionar(new Ponto(posicao, 2, false));
							index1 = elem_info.Length;
						}
					}
					catch
					{
						if (!multiplo)
							index1 = elem_info.Length;
					}
				}
				index1 += 3;
			}
			return pontoCollection;
		}

		private PontoCollection GerarPontosLRS2D(Decimal[] elem_info, Decimal[] ordinates, bool multiplo)
		{
			PontoCollection pontoCollection = new PontoCollection();
			int index1 = 0;
			while (index1 < elem_info.Length)
			{
				if (elem_info[index1 + 1] == new Decimal(1))
				{
					try
					{
						int int32 = Convert.ToInt32(elem_info[index1]);
						if (multiplo)
						{
							int num = index1 < elem_info.Length - 3 ? Convert.ToInt32((elem_info[index1 + 3]) - 1) : ordinates.Length - 1;
							int index2 = int32;
							while (index2 < num)
							{
								pontoCollection.Adicionar(new Ponto(new Posicao(ordinates[index2], ordinates[index2 + 1], true)
								{
									M = ordinates[index2 + 2]
								}, 2, true));
								index2 += 3;
							}
						}
						else
						{
							pontoCollection.Adicionar(new Ponto(new Posicao(ordinates[int32], ordinates[int32 + 1], true)
							{
								M = ordinates[int32 + 2]
							}, 2, true));
							index1 = elem_info.Length;
						}
					}
					catch
					{
						if (!multiplo)
							index1 = elem_info.Length;
					}
				}
				index1 += 3;
			}
			return pontoCollection;
		}

		private PontoCollection GerarPontos3D(Decimal[] elem_info, Decimal[] ordinates, bool multiplo)
		{
			PontoCollection pontoCollection = new PontoCollection();
			int index1 = 0;
			while (index1 < elem_info.Length)
			{
				if (elem_info[index1 + 1] == new Decimal(1))
				{
					try
					{
						int int32 = Convert.ToInt32(elem_info[index1]);
						if (multiplo)
						{
							int num = index1 < elem_info.Length - 3 ? Convert.ToInt32((elem_info[index1 + 3]) - 1) : ordinates.Length - 1;
							int index2 = int32;
							while (index2 < num)
							{
								Posicao posicao = new Posicao(ordinates[index2], ordinates[index2 + 1], ordinates[index2 + 2], false);
								pontoCollection.Adicionar(new Ponto(posicao, 3, false));
								index2 += 3;
							}
						}
						else
						{
							Posicao posicao = new Posicao(ordinates[int32], ordinates[int32 + 1], ordinates[int32 + 2], false);
							pontoCollection.Adicionar(new Ponto(posicao, 3, false));
							index1 = elem_info.Length;
						}
					}
					catch
					{
						if (!multiplo)
							index1 = elem_info.Length;
					}
				}
				index1 += 3;
			}
			return pontoCollection;
		}

		private PontoCollection GerarPontosLRS3D(Decimal[] elem_info, Decimal[] ordinates, int eixoLrs, bool multiplo)
		{
			PontoCollection pontoCollection = new PontoCollection();
			int index1 = 0;
			int num1 = eixoLrs - 1;
			int num2 = 5 - num1;
			while (index1 < elem_info.Length)
			{
				if (elem_info[index1 + 1] == new Decimal(1))
				{
					try
					{
						int int32 = Convert.ToInt32(elem_info[index1]);
						if (multiplo)
						{
							int num3 = index1 < elem_info.Length - 3 ? Convert.ToInt32((elem_info[index1 + 3]) - 1) : ordinates.Length - 1;
							int index2 = int32;
							while (index2 < num3)
							{
								pontoCollection.Adicionar(new Ponto(new Posicao(ordinates[index2], ordinates[index2 + 1], ordinates[index2 + num2], true)
								{
									M = ordinates[index2 + num1]
								}, 3, true));
								index2 += 4;
							}
						}
						else
						{
							pontoCollection.Adicionar(new Ponto(new Posicao(ordinates[int32], ordinates[int32 + 1], ordinates[int32 + num2], true)
							{
								M = ordinates[int32 + num1]
							}, 3, true));
							index1 = elem_info.Length;
						}
					}
					catch
					{
						if (!multiplo)
							index1 = elem_info.Length;
					}
				}
				index1 += 3;
			}
			return pontoCollection;
		}

		private PosicaoCollection ExtrairPosicoes(Decimal[] ordinates, int inicio, int fim, int dimensoes, int eixoLrs)
		{
			bool flag = eixoLrs != 0;
			try
			{
				switch (dimensoes)
				{
					case 2:
						return this.ExtrairPosicoes2D(ordinates, inicio, fim);
					case 3:
						if (flag)
							return this.ExtrairPosicoesLRS2D(ordinates, inicio, fim);
						return this.ExtrairPosicoes3D(ordinates, inicio, fim);
					case 4:
						return this.ExtrairPosicoesLRS3D(ordinates, inicio, fim, eixoLrs);
					default:
						return (PosicaoCollection)null;
				}
			}
			catch
			{
				return (PosicaoCollection)null;
			}
		}

		private PosicaoCollection ExtrairPosicoes2D(Decimal[] ordinates, int inicio, int fim)
		{
			PosicaoCollection posicaoCollection = new PosicaoCollection();
			int index = inicio;
			while (index < fim)
			{
				posicaoCollection.Adicionar(new Posicao(ordinates[index], ordinates[index + 1], false));
				index += 2;
			}
			return posicaoCollection;
		}

		private PosicaoCollection ExtrairPosicoesLRS2D(Decimal[] ordinates, int inicio, int fim)
		{
			PosicaoCollection posicaoCollection = new PosicaoCollection();
			int index = inicio;
			while (index < fim)
			{
				posicaoCollection.Adicionar(new Posicao(ordinates[index], ordinates[index + 1], true)
				{
					M = ordinates[index + 2]
				});
				index += 3;
			}
			return posicaoCollection;
		}

		private PosicaoCollection ExtrairPosicoes3D(Decimal[] ordinates, int inicio, int fim)
		{
			PosicaoCollection posicaoCollection = new PosicaoCollection();
			int index = inicio;
			while (index < fim)
			{
				posicaoCollection.Adicionar(new Posicao(ordinates[index], ordinates[index + 1], ordinates[index + 2], false));
				index += 3;
			}
			return posicaoCollection;
		}

		private PosicaoCollection ExtrairPosicoesLRS3D(Decimal[] ordinates, int inicio, int fim, int eixoLrs)
		{
			PosicaoCollection posicaoCollection = new PosicaoCollection();
			int num1 = eixoLrs - 1;
			int num2 = 5 - num1;
			int index = inicio;
			while (index < fim)
			{
				posicaoCollection.Adicionar(new Posicao(ordinates[index], ordinates[index + 1], ordinates[index + num2], true)
				{
					M = ordinates[index + num1]
				});
				index += 4;
			}
			return posicaoCollection;
		}

		private SegmentoLinha GerarSegmento(Decimal[] ordinates, int inicio, int fim, int tipo, int dimensoes, int eixoLrs)
		{
			try
			{
				SegmentoLinha segmentoLinha;
				switch (tipo)
				{
					case 1:
						segmentoLinha = (SegmentoLinha)new SegmentoLinhaReto();
						break;
					case 2:
						segmentoLinha = (SegmentoLinha)new SegmentoLinhaArco();
						break;
					case 3:
						segmentoLinha = (SegmentoLinha)new SegmentoLinhaReto();
						break;
					case 4:
						segmentoLinha = (SegmentoLinha)new SegmentoLinhaCirculo();
						break;
					default:
						return (SegmentoLinha)null;
				}
				PosicaoCollection posicoes = this.ExtrairPosicoes(ordinates, inicio, fim, dimensoes, eixoLrs);
				if (posicoes == null || posicoes.Count <= 1 || tipo == 2 && posicoes.Count % 2 == 0 || (tipo == 3 && posicoes.Count != 2 || tipo == 4 && posicoes.Count != 3))
					return (SegmentoLinha)null;
				segmentoLinha.Posicoes.Adicionar(posicoes);
				return segmentoLinha;
			}
			catch
			{
				return (SegmentoLinha)null;
			}
		}

		private LinhaCollection GerarLinhas(Decimal[] elem_info, Decimal[] ordinates, int dimensoes, int eixoLrs, bool multiplo)
		{
			LinhaCollection linhaCollection = new LinhaCollection();
			int index1 = 0;
			bool ehLrs = eixoLrs != 0;
			while (index1 < elem_info.Length)
			{
				if (elem_info[index1 + 1] == new Decimal(4))
				{
					try
					{
						int int32_1 = Convert.ToInt32(elem_info[index1 + 2]);
						Linha linha = new Linha(ehLrs ? dimensoes - 1 : dimensoes, ehLrs);
						for (int index2 = 0; index2 < int32_1; ++index2)
						{
							index1 += 3;
							if (elem_info[index1 + 1] != new Decimal(2))
								throw new Exception("Tipo heterogêneo de linha errado");
							int int32_2 = Convert.ToInt32(elem_info[index1]);
							int fim = index1 < elem_info.Length - 3 ? Convert.ToInt32(elem_info[index1 + 3]) - (index2 == int32_1 - 1 ? 1 : -1) : ordinates.Length - 1;
							SegmentoLinha segmento = this.GerarSegmento(ordinates, int32_2, fim, Convert.ToInt32(elem_info[index1 + 2]), dimensoes, eixoLrs);
							if (segmento == null)
								throw new Exception("Falha ao ler um segmento");
							linha.Segmentos.Adicionar(segmento);
						}
						linhaCollection.Adicionar(linha);
					}
					catch
					{
					}
					if (!multiplo)
						return linhaCollection;
				}
				else if (elem_info[index1 + 1] == new Decimal(2))
				{
					try
					{
						int int32 = Convert.ToInt32(elem_info[index1]);
						int fim = index1 < elem_info.Length - 3 ? Convert.ToInt32((elem_info[index1 + 3]) - 1) : ordinates.Length - 1;
						SegmentoLinha segmento = this.GerarSegmento(ordinates, int32, fim, Convert.ToInt32(elem_info[index1 + 2]), dimensoes, eixoLrs);
						Linha linha = new Linha(ehLrs ? dimensoes - 1 : dimensoes, ehLrs);
						if (segmento == null)
							throw new Exception("Falha ao ler um segmento");
						linha.Segmentos.Adicionar(segmento);
						linhaCollection.Adicionar(linha);
					}
					catch
					{
					}
					if (!multiplo)
						return linhaCollection;
				}
				else if (elem_info[index1 + 1] == new Decimal(1005) || elem_info[index1 + 1] == new Decimal(2005))
					index1 += 3 * Convert.ToInt32(elem_info[index1 + 2]);
				index1 += 3;
			}
			return linhaCollection;
		}

		private PoligonoCollection GerarPoligonos(Decimal[] elem_info, Decimal[] ordinates, int dimensoes, int eixoLrs, bool multiplo)
		{
			PoligonoCollection poligonoCollection = new PoligonoCollection();
			int triplet = 0;
			bool ehLrs = eixoLrs != 0;
			while (triplet < elem_info.Length)
			{
				if (elem_info[triplet + 1] == new Decimal(1005) || elem_info[triplet + 1] == new Decimal(1003))
				{
					try
					{
						Anel anel1 = this.GerarAnel(elem_info, ordinates, dimensoes, eixoLrs, ref triplet);
						Poligono poligono = new Poligono(ehLrs ? dimensoes - 1 : dimensoes, ehLrs);
						if (anel1 == null)
							throw new Exception("Falha ao ler o anel externo");
						poligono.Aneis.Adicionar(anel1);
						triplet += 3;
						while (triplet < elem_info.Length && (elem_info[triplet + 1] == new Decimal(2005) || elem_info[triplet + 1] == new Decimal(2003)))
						{
							Anel anel2 = this.GerarAnel(elem_info, ordinates, dimensoes, eixoLrs, ref triplet);
							if (anel2 == null)
								throw new Exception("Falha ao ler o anel interno");
							poligono.Aneis.Adicionar(anel2);
							triplet += 3;
						}
						triplet -= 3;
						poligonoCollection.Adicionar(poligono);
					}
					catch
					{
					}
					if (!multiplo)
						return poligonoCollection;
				}
				triplet += 3;
			}
			return poligonoCollection;
		}

		private Anel GerarAnel(Decimal[] elem_info, Decimal[] ordinates, int dimensoes, int eixoLrs, ref int triplet)
		{
			Anel anel = (Anel)null;
			bool flag = eixoLrs != 0;
			if (elem_info[triplet + 1] == new Decimal(1005) || elem_info[triplet + 1] == new Decimal(2005))
			{
				try
				{
					int int32_1 = Convert.ToInt32(elem_info[triplet + 2]);
					anel = new Anel();
					for (int index = 0; index < int32_1; ++index)
					{
						triplet += 3;
						if (elem_info[triplet + 1] != new Decimal(2))
							throw new Exception("Tipo heterogêneo de anel errado");
						int int32_2 = Convert.ToInt32(elem_info[triplet]);
						int fim = triplet < elem_info.Length - 3 ? Convert.ToInt32(elem_info[triplet + 3]) - (index == int32_1 - 1 ? 1 : -1) : ordinates.Length - 1;
						SegmentoLinha segmento = this.GerarSegmento(ordinates, int32_2, fim, Convert.ToInt32(elem_info[triplet + 2]), dimensoes, eixoLrs);
						if (segmento == null)
							throw new Exception("Falha ao ler um segmento");
						anel.Segmentos.Adicionar(segmento);
					}
				}
				catch
				{
					anel = (Anel)null;
				}
			}
			else if (elem_info[triplet + 1] == new Decimal(1003) || elem_info[triplet + 1] == new Decimal(2003))
			{
				try
				{
					int int32 = Convert.ToInt32(elem_info[triplet]);
					int fim = triplet < elem_info.Length - 3 ? Convert.ToInt32((elem_info[triplet + 3]) - 1) : ordinates.Length - 1;
					SegmentoLinha segmento = this.GerarSegmento(ordinates, int32, fim, Convert.ToInt32(elem_info[triplet + 2]), dimensoes, eixoLrs);
					if (segmento == null)
						throw new Exception("Falha ao ler um segmento");
					if (elem_info[triplet + 2] == new Decimal(3))
					{
						Posicao[] posicaoArray = new Posicao[4]
						{
			  segmento.Posicoes[0],
			  null,
			  segmento.Posicoes[1],
			  null
						};
						posicaoArray[1] = new Posicao(posicaoArray[0].Dimensoes, posicaoArray[0].EhLrs);
						posicaoArray[1].X = posicaoArray[2].X;
						posicaoArray[1].Y = posicaoArray[0].Y;
						if (posicaoArray[1].Dimensoes == 3)
							posicaoArray[1].Z = posicaoArray[0].Z;
						posicaoArray[3] = new Posicao(posicaoArray[2].Dimensoes, posicaoArray[2].EhLrs);
						posicaoArray[3].X = posicaoArray[0].X;
						posicaoArray[3].Y = posicaoArray[2].Y;
						if (posicaoArray[3].Dimensoes == 3)
							posicaoArray[3].Z = posicaoArray[2].Z;
						segmento = (SegmentoLinha)new SegmentoLinhaReto();
						segmento.Posicoes.Adicionar(posicaoArray[0]);
						if (elem_info[triplet + 1] == new Decimal(1003))
						{
							segmento.Posicoes.Adicionar(posicaoArray[1]);
							segmento.Posicoes.Adicionar(posicaoArray[2]);
							segmento.Posicoes.Adicionar(posicaoArray[3]);
						}
						else
						{
							segmento.Posicoes.Adicionar(posicaoArray[3]);
							segmento.Posicoes.Adicionar(posicaoArray[2]);
							segmento.Posicoes.Adicionar(posicaoArray[1]);
						}
						segmento.Posicoes.Adicionar(posicaoArray[0]);
					}
					anel = new Anel();
					anel.Segmentos.Adicionar(segmento);
				}
				catch
				{
				}
			}
			return anel;
		}
	}
}
