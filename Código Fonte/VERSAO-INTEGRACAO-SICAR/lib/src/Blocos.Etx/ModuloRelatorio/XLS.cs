using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio
{
	[Serializable]
	public class XLS
	{
		#region Elementos XML

		public const string MSO_APPLICATION = "mso-application";
		public const string PROGID = "progid=\"Excel.Sheet\"";

		// Workbook
		public const string TAG_PASTA = "Workbook";
		public const string TAG_NAMESPACE = "xmlns";

		public const string PREFIXO_EXCEL = "x";
		public const string PREFIXO_ESTILO = "ss";

		public const string VALOR_NAMESPACE_ESTILO = "urn:schemas-microsoft-com:office:spreadsheet";
		public const string VALOR_NAMESPACE_EXCEL = "urn:schemas-microsoft-com:office:excel";

		public class Estilos
		{
			#region Tags

			// Styles
			public const string TAG_ESTILOS = "Styles";

			// Style
			public const string TAG_ESTILO = "Style";
			public const string TAG_ESTILO_ID = "ID";
			public const string TAG_ESTILO_NOME = "Name";
			public const string TAG_ESTILO_PAI = "Parent";

			// Aligment
			public const string TAG_ALINHAMENTO = "Alignment";
			public const string TAG_ALINHAMENTO_VERTICAL = "Vertical";
			public const string TAG_ALINHAMENTO_HORIZONTAL = "Horizontal";

			// Borders
			public const string TAG_BORDAS = "Borders";
			public const string TAG_BORDA = "Border";
			public const string TAG_BORDA_POSICAO = "Position";
			public const string TAG_BORDA_ESTILO_LINHA = "LineStyle";
			public const string TAG_BORDA_PESO = "Weight";

			// Font
			public const string TAG_FONTE = "Font";
			public const string TAG_FONTE_NOME = "FontName";
			public const string TAG_FONTE_FAMILIA = "Family";
			public const string TAG_FONTE_TAMANHO = "Size";
			public const string TAG_FONTE_COR = "Color";
			public const string TAG_FONTE_NEGRITO = "Bold";

			// Interior
			public const string TAG_INTERIOR = "Interior";
			public const string TAG_INTERIOR_COR = "Color";
			public const string TAG_INTERIOR_MODELO = "Pattern";

			// NumberFormat
			public const string TAG_FORMATACAO_NUMERO = "NumberFormat";
			public const string TAG_FORMATACAO_NUMERO_FORMATO = "Format";

			#endregion

			#region Valores

			#region Padrão

			// Style
			public const string PADRAO_VALOR_ESTILO_ID = "Default";
			public const string PADRAO_VALOR_ESTILO_NOME = "Normal";

			// Aligment
			public const string PADRAO_VALOR_ALINHAMENTO_VERTICAL = "Bottom";

			// Font
			public const string PADRAO_VALOR_FONTE_NOME = "Arial";
			public const string PADRAO_VALOR_FONTE_FAMILIA = "Swiss";
			public const string PADRAO_VALOR_FONTE_TAMANHO = "12";
			public const string PADRAO_VALOR_FONTE_COR = "#000000";

			#endregion

			#region Cabeçalho

			// Style
			public const string CABECALHO_VALOR_ESTILO_ID = "Cabecalho";
			public const string CABECALHO_VALOR_ESTILO_NOME = "Cabecalho";
			public const string CABECALHO_VALOR_ESTILO_PAI = "Default";

			// Aligment
			public const string CABECALHO_VALOR_ALINHAMENTO_HORIZONTAL = "Center";

			// Font
			public const string CABECALHO_VALOR_FONTE_TAMANHO = "14";
			public const string CABECALHO_VALOR_FONTE_COR = "#FFFFFF";
			public const string CABECALHO_VALOR_FONTE_NEGRITO = "1";

			// Interior
			public const string CABECALHO_VALOR_INTERIOR_COR = "#75923C";
			public const string CABECALHO_VALOR_INTERIOR_MODELO = "Solid";

			#endregion

			#region Coluna

			// Style
			public const string COLUNA_VALOR_ESTILO_ID = "Coluna";
			public const string COLUNA_VALOR_ESTILO_NOME = "Coluna";
			public const string COLUNA_VALOR_ESTILO_PAI = "Default";

			// Borders
			public const string COLUNA_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string COLUNA_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string COLUNA_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string COLUNA_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string COLUNA_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string COLUNA_VALOR_BORDA_PESO = "1";

			// Font
			public const string COLUNA_VALOR_FONTE_COR = "#FFFFFF";
			public const string COLUNA_VALOR_FONTE_NEGRITO = "1";

			// Interior
			public const string COLUNA_VALOR_INTERIOR_COR = "#75923C";
			public const string COLUNA_VALOR_INTERIOR_MODELO = "Solid";

			#endregion

			#region Texto Claro

			// Style
			public const string TEXTO_CLARO_VALOR_ESTILO_ID = "TextoClaro";
			public const string TEXTO_CLARO_VALOR_ESTILO_NOME = "TextoClaro";
			public const string TEXTO_CLARO_VALOR_ESTILO_PAI = "Default";

			// Borders
			public const string TEXTO_CLARO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string TEXTO_CLARO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string TEXTO_CLARO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string TEXTO_CLARO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string TEXTO_CLARO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string TEXTO_CLARO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string TEXTO_CLARO_VALOR_INTERIOR_COR = "#EAF1DD";
			public const string TEXTO_CLARO_VALOR_INTERIOR_MODELO = "Solid";

			#endregion

			#region Número Extenso Claro

			// Style
			public const string NUMERO_EXTENSO_CLARO_VALOR_ESTILO_ID = "NumeroExtensoClaro";
			public const string NUMERO_EXTENSO_CLARO_VALOR_ESTILO_NOME = "NumeroExtensoClaro";
			public const string NUMERO_EXTENSO_CLARO_VALOR_ESTILO_PAI = "Default";

			// Borders
			public const string NUMERO_EXTENSO_CLARO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string NUMERO_EXTENSO_CLARO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string NUMERO_EXTENSO_CLARO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string NUMERO_EXTENSO_CLARO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string NUMERO_EXTENSO_CLARO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string NUMERO_EXTENSO_CLARO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string NUMERO_EXTENSO_CLARO_VALOR_INTERIOR_COR = "#EAF1DD";
			public const string NUMERO_EXTENSO_CLARO_VALOR_INTERIOR_MODELO = "Solid";

			// NumberFormat
			public const string NUMERO_EXTENSO_CLARO_VALOR_FORMATACAO_NUMERO_FORMATO = "@";

			#endregion

			#region Número Claro

			// Style
			public const string NUMERO_CLARO_VALOR_ESTILO_ID = "NumeroClaro";
			public const string NUMERO_CLARO_VALOR_ESTILO_NOME = "NumeroClaro";
			public const string NUMERO_CLARO_VALOR_ESTILO_PAI = "Default";

			// Aligment
			public const string NUMERO_CLARO_VALOR_ALINHAMENTO_HORIZONTAL = "Right";

			// Borders
			public const string NUMERO_CLARO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string NUMERO_CLARO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string NUMERO_CLARO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string NUMERO_CLARO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string NUMERO_CLARO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string NUMERO_CLARO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string NUMERO_CLARO_VALOR_INTERIOR_COR = "#EAF1DD";
			public const string NUMERO_CLARO_VALOR_INTERIOR_MODELO = "Solid";

			// NumberFormat
			public const string NUMERO_CLARO_VALOR_FORMATACAO_NUMERO_FORMATO = "0";

			#endregion

			#region Número Claro - Dois Dígitos

			// Style
			public const string NUMERO2_CLARO_VALOR_ESTILO_ID = "Numero2Claro";
			public const string NUMERO2_CLARO_VALOR_ESTILO_NOME = "Numero2Claro";
			public const string NUMERO2_CLARO_VALOR_ESTILO_PAI = "Default";

			// Aligment
			public const string NUMERO2_CLARO_VALOR_ALINHAMENTO_HORIZONTAL = "Right";

			// Borders
			public const string NUMERO2_CLARO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string NUMERO2_CLARO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string NUMERO2_CLARO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string NUMERO2_CLARO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string NUMERO2_CLARO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string NUMERO2_CLARO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string NUMERO2_CLARO_VALOR_INTERIOR_COR = "#EAF1DD";
			public const string NUMERO2_CLARO_VALOR_INTERIOR_MODELO = "Solid";

			// NumberFormat
			public const string NUMERO2_CLARO_VALOR_FORMATACAO_NUMERO_FORMATO = "Standard";

			#endregion

			#region Número Claro - Quatro Dígitos

			// Style
			public const string NUMERO4_CLARO_VALOR_ESTILO_ID = "Numero4Claro";
			public const string NUMERO4_CLARO_VALOR_ESTILO_NOME = "Numero4Claro";
			public const string NUMERO4_CLARO_VALOR_ESTILO_PAI = "Default";

			// Aligment
			public const string NUMERO4_CLARO_VALOR_ALINHAMENTO_HORIZONTAL = "Right";

			// Borders
			public const string NUMERO4_CLARO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string NUMERO4_CLARO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string NUMERO4_CLARO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string NUMERO4_CLARO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string NUMERO4_CLARO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string NUMERO4_CLARO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string NUMERO4_CLARO_VALOR_INTERIOR_COR = "#EAF1DD";
			public const string NUMERO4_CLARO_VALOR_INTERIOR_MODELO = "Solid";

			// NumberFormat
			public const string NUMERO4_CLARO_VALOR_FORMATACAO_NUMERO_FORMATO = "#,##0.0000";

			#endregion

			#region Data Claro

			// Style
			public const string DATA_CLARO_VALOR_ESTILO_ID = "DataClaro";
			public const string DATA_CLARO_VALOR_ESTILO_NOME = "DataClaro";
			public const string DATA_CLARO_VALOR_ESTILO_PAI = "Default";

			// Borders
			public const string DATA_CLARO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string DATA_CLARO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string DATA_CLARO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string DATA_CLARO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string DATA_CLARO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string DATA_CLARO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string DATA_CLARO_VALOR_INTERIOR_COR = "#EAF1DD";
			public const string DATA_CLARO_VALOR_INTERIOR_MODELO = "Solid";

			// NumberFormat
			public const string DATA_CLARO_VALOR_FORMATACAO_NUMERO_FORMATO = "dd/mm/yyyy\\ hh:mm:ss";

			#endregion

			#region Data Curta Claro

			// Style
			public const string DATA_CURTA_CLARO_VALOR_ESTILO_ID = "DataCurtaClaro";
			public const string DATA_CURTA_CLARO_VALOR_ESTILO_NOME = "DataCurtaClaro";
			public const string DATA_CURTA_CLARO_VALOR_ESTILO_PAI = "Default";

			// Borders
			public const string DATA_CURTA_CLARO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string DATA_CURTA_CLARO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string DATA_CURTA_CLARO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string DATA_CURTA_CLARO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string DATA_CURTA_CLARO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string DATA_CURTA_CLARO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string DATA_CURTA_CLARO_VALOR_INTERIOR_COR = "#EAF1DD";
			public const string DATA_CURTA_CLARO_VALOR_INTERIOR_MODELO = "Solid";

			// NumberFormat
			public const string DATA_CURTA_CLARO_VALOR_FORMATACAO_NUMERO_FORMATO = "Short Date";

			#endregion

			#region Texto Escuro

			// Style
			public const string TEXTO_ESCURO_VALOR_ESTILO_ID = "TextoEscuro";
			public const string TEXTO_ESCURO_VALOR_ESTILO_NOME = "TextoEscuro";
			public const string TEXTO_ESCURO_VALOR_ESTILO_PAI = "Default";

			// Borders
			public const string TEXTO_ESCURO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string TEXTO_ESCURO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string TEXTO_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string TEXTO_ESCURO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string TEXTO_ESCURO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string TEXTO_ESCURO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string TEXTO_ESCURO_VALOR_INTERIOR_COR = "#C2D69A";
			public const string TEXTO_ESCURO_VALOR_INTERIOR_MODELO = "Solid";

			#endregion

			#region Número Extenso Escuro

			// Style
			public const string NUMERO_EXTENSO_ESCURO_VALOR_ESTILO_ID = "NumeroExtensoEscuro";
			public const string NUMERO_EXTENSO_ESCURO_VALOR_ESTILO_NOME = "NumeroExtensoEscuro";
			public const string NUMERO_EXTENSO_ESCURO_VALOR_ESTILO_PAI = "Default";

			// Borders
			public const string NUMERO_EXTENSO_ESCURO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string NUMERO_EXTENSO_ESCURO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string NUMERO_EXTENSO_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string NUMERO_EXTENSO_ESCURO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string NUMERO_EXTENSO_ESCURO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string NUMERO_EXTENSO_ESCURO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string NUMERO_EXTENSO_ESCURO_VALOR_INTERIOR_COR = "#C2D69A";
			public const string NUMERO_EXTENSO_ESCURO_VALOR_INTERIOR_MODELO = "Solid";

			// NumberFormat
			public const string NUMERO_EXTENSO_ESCURO_VALOR_FORMATACAO_NUMERO_FORMATO = "@";

			#endregion

			#region Número Escuro

			// Style
			public const string NUMERO_ESCURO_VALOR_ESTILO_ID = "NumeroEscuro";
			public const string NUMERO_ESCURO_VALOR_ESTILO_NOME = "NumeroEscuro";
			public const string NUMERO_ESCURO_VALOR_ESTILO_PAI = "Default";

			// Aligment
			public const string NUMERO_ESCURO_VALOR_ALINHAMENTO_HORIZONTAL = "Right";

			// Borders
			public const string NUMERO_ESCURO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string NUMERO_ESCURO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string NUMERO_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string NUMERO_ESCURO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string NUMERO_ESCURO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string NUMERO_ESCURO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string NUMERO_ESCURO_VALOR_INTERIOR_COR = "#C2D69A";
			public const string NUMERO_ESCURO_VALOR_INTERIOR_MODELO = "Solid";

			// NumberFormat
			public const string NUMERO_ESCURO_VALOR_FORMATACAO_NUMERO_FORMATO = "0";

			#endregion

			#region Número Escuro - Dois Dígitos

			// Style
			public const string NUMERO2_ESCURO_VALOR_ESTILO_ID = "Numero2Escuro";
			public const string NUMERO2_ESCURO_VALOR_ESTILO_NOME = "Numero2Escuro";
			public const string NUMERO2_ESCURO_VALOR_ESTILO_PAI = "Default";

			// Aligment
			public const string NUMERO2_ESCURO_VALOR_ALINHAMENTO_HORIZONTAL = "Right";

			// Borders
			public const string NUMERO2_ESCURO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string NUMERO2_ESCURO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string NUMERO2_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string NUMERO2_ESCURO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string NUMERO2_ESCURO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string NUMERO2_ESCURO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string NUMERO2_ESCURO_VALOR_INTERIOR_COR = "#C2D69A";
			public const string NUMERO2_ESCURO_VALOR_INTERIOR_MODELO = "Solid";

			// NumberFormat
			public const string NUMERO2_ESCURO_VALOR_FORMATACAO_NUMERO_FORMATO = "Standard";

			#endregion

			#region Número Escuro - Quatro Dígitos

			// Style
			public const string NUMERO4_ESCURO_VALOR_ESTILO_ID = "Numero4Escuro";
			public const string NUMERO4_ESCURO_VALOR_ESTILO_NOME = "Numero4Escuro";
			public const string NUMERO4_ESCURO_VALOR_ESTILO_PAI = "Default";

			// Aligment
			public const string NUMERO4_ESCURO_VALOR_ALINHAMENTO_HORIZONTAL = "Right";

			// Borders
			public const string NUMERO4_ESCURO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string NUMERO4_ESCURO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string NUMERO4_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string NUMERO4_ESCURO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string NUMERO4_ESCURO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string NUMERO4_ESCURO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string NUMERO4_ESCURO_VALOR_INTERIOR_COR = "#C2D69A";
			public const string NUMERO4_ESCURO_VALOR_INTERIOR_MODELO = "Solid";

			// NumberFormat
			public const string NUMERO4_ESCURO_VALOR_FORMATACAO_NUMERO_FORMATO = "#,##0.0000";

			#endregion

			#region Data Escuro

			// Style
			public const string DATA_ESCURO_VALOR_ESTILO_ID = "DataEscuro";
			public const string DATA_ESCURO_VALOR_ESTILO_NOME = "DataEscuro";
			public const string DATA_ESCURO_VALOR_ESTILO_PAI = "Default";

			// Borders
			public const string DATA_ESCURO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string DATA_ESCURO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string DATA_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string DATA_ESCURO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string DATA_ESCURO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string DATA_ESCURO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string DATA_ESCURO_VALOR_INTERIOR_COR = "#C2D69A";
			public const string DATA_ESCURO_VALOR_INTERIOR_MODELO = "Solid";

			// NumberFormat
			public const string DATA_ESCURO_VALOR_FORMATACAO_NUMERO_FORMATO = "dd/mm/yyyy\\ hh:mm:ss";

			#endregion

			#region Data Curta Escuro

			// Style
			public const string DATA_CURTA_ESCURO_VALOR_ESTILO_ID = "DataCurtaEscuro";
			public const string DATA_CURTA_ESCURO_VALOR_ESTILO_NOME = "DataCurtaEscuro";
			public const string DATA_CURTA_ESCURO_VALOR_ESTILO_PAI = "Default";

			// Borders
			public const string DATA_CURTA_ESCURO_VALOR_BORDA_POSICAO_TOPO = "Top";
			public const string DATA_CURTA_ESCURO_VALOR_BORDA_POSICAO_BAIXO = "Bottom";
			public const string DATA_CURTA_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO = "Left";
			public const string DATA_CURTA_ESCURO_VALOR_BORDA_POSICAO_DIREITO = "Right";
			public const string DATA_CURTA_ESCURO_VALOR_BORDA_ESTILO_LINHA = "Continuous";
			public const string DATA_CURTA_ESCURO_VALOR_BORDA_PESO = "1";

			// Interior
			public const string DATA_CURTA_ESCURO_VALOR_INTERIOR_COR = "#C2D69A";
			public const string DATA_CURTA_ESCURO_VALOR_INTERIOR_MODELO = "Solid";

			// NumberFormat
			public const string DATA_CURTA_ESCURO_VALOR_FORMATACAO_NUMERO_FORMATO = "Short Date";

			#endregion

			#endregion
		}

		public class Planilha
		{ 
			#region Tags

			public const string TAG = "Worksheet";
			public const string TAG_NOME = "Name";
			public const string TAG_NOMES = "Names";
			public const string TAG_NOMES_ALCANCE = "NamedRange";
			public const string TAG_NOMES_ALCANCE_NOME = "Name";
			public const string TAG_NOMES_ALCANCE_REFERENCIA = "RefersTo";
			public const string TAG_NOMES_ALCANCE_INVISIVEL = "Hidden";
			public const string TAG_FILTRO_AUTOMATICO = "AutoFilter";
			public const string TAG_FILTRO_AUTOMATICO_ALCANCE = "Range";

			#endregion

			#region Valores

			public const string VALOR_NOME = "Relatório";
			public const string VALOR_NOMES_ALCANCE_NOME = "_FilterDatabase";
			public const string VALOR_NOMES_ALCANCE_REFERENCIA = "=Relatório!R2C1:R2C";
			public const string VALOR_NOMES_ALCANCE_INVISIVEL = "1";
			public const string VALOR_FILTRO_AUTOMATICO_ALCANCE = "R2C1:R2C";

			#endregion
		}

		public class Tabela
		{
			#region Tags

			// Table
			public const string TAG = "Table";
			public const string TAG_TABELA_COLUNAS_EXPANDIDAS = "ExpandedColumnCount";
			public const string TAG_TABELA_COLUNAS_PREENCHIDAS = "FullColumns";
			public const string TAG_TABELA_LINHAS_EXPANDIDAS = "ExpandedRowCount";
			public const string TAG_TABELA_LINHAS_PREENCHIDAS = "FullRows";
			public const string TAG_TABELA_LINHA_TAMANHO_PADRAO = "DefaultRowHeight";

			// Column
			public const string TAG_COLUNA = "Column";
			public const string TAG_COLUNA_TAMANHO = "Width";
			public const string TAG_COLUNA_FORMATACAO = "Span";
			public const string TAG_COLUNA_TAMANHO_AUTOMATICO = "AutoFitWidth";

			// Row
			public const string TAG_LINHA = "Row";
			public const string TAG_LINHA_ALTURA = "Height";

			// Cell
			public const string TAG_CELULA = "Cell";
			public const string TAG_CELULA_MESCLA = "MergeAcross";
			public const string TAG_CELULA_ESTILO = "StyleID";

			// Data
			public const string TAG_DADO = "Data";
			public const string TAG_DADO_TIPO = "Type";

			#endregion

			#region Valores

			// Table
			public const string VALOR_TABELA_COLUNAS_PREENCHIDAS = "1";
			public const string VALOR_TABELA_LINHAS_PREENCHIDAS = "1";
			public const string VALOR_TABELA_LINHA_TAMANHO_PADRAO = "15";
			public const string VALOR_COLUNA_TAMANHO_AUTOMATICO = "0";

			// Row
			public const string VALOR_LINHA_ALTURA_CABECALHO = "18";

			#endregion
		}

		#endregion

		#region Enumerados

		public enum EDadoTipo
		{
			String,
			Number,
			Number2,
			Number4,
			NumberLarge,
			DateTime,
			DateTimeShort
		}

		#endregion

		#region Atributos

		private XmlTextWriter _xmlEscritor;
		private string _tituloCabecalho;
		private string[] _colunaNomes;
		private EDadoTipo[] _colunaTipos;
		private uint[] _colunaTamanhos;
		private DataTable _tabelaDados;

		#endregion

		#region Propriedades

		public string TituloCabecalho
		{
			get { return _tituloCabecalho; }
			set { _tituloCabecalho = value; }
		}

		public string[] ColunaNomes
		{
			get { return _colunaNomes; }
			set { _colunaNomes = value; }
		}

		public EDadoTipo[] ColunaTipos
		{
			get { return _colunaTipos; }
			set { _colunaTipos = value; }
		}

		public uint[] ColunaTamanhos
		{
			get { return _colunaTamanhos; }
			set { _colunaTamanhos = value; }
		}

		public DataTable TabelaDados
		{
			get { return _tabelaDados; }
			set { _tabelaDados = value; }
		}

		#endregion

		#region Construtor

		public XLS(string tituloCabecalho, DataTable tabelaDados, EDadoTipo[] colunaTipos)
		{
			_tituloCabecalho = tituloCabecalho;
			_tabelaDados = tabelaDados;

			if (tabelaDados != null && tabelaDados.Rows != null && tabelaDados.Rows.Count > 0)
			{
				_colunaNomes = new string[tabelaDados.Columns.Count];

				for (int i = 0; i < tabelaDados.Columns.Count; ++i)
				{
					_colunaNomes[i] = tabelaDados.Columns[i].ColumnName;
				}

				if (colunaTipos != null && _colunaNomes.Length == colunaTipos.Length)
				{
					_colunaTipos = colunaTipos;
				}
			}
		}

		public XLS(string tituloCabecalho, DataTable tabelaDados, string[] colunaNomes, EDadoTipo[] colunaTipos)
		{
			_tituloCabecalho = tituloCabecalho;
			_tabelaDados = tabelaDados;

			if (tabelaDados != null && tabelaDados.Rows != null && tabelaDados.Rows.Count > 0 && colunaNomes != null && tabelaDados.Columns.Count == colunaNomes.Length
				 && colunaTipos != null && colunaNomes.Length == colunaTipos.Length)
			{
				_colunaNomes = colunaNomes;
				_colunaTipos = colunaTipos;
			}
		}

		public XLS(string tituloCabecalho, DataTable tabelaDados, string[] colunaNomes, EDadoTipo[] colunaTipos, uint[] colunaTamanhos)
		{
			_tituloCabecalho = tituloCabecalho;
			_tabelaDados = tabelaDados;

			if (tabelaDados != null && tabelaDados.Rows != null && tabelaDados.Rows.Count > 0 && colunaNomes != null && tabelaDados.Rows.Count == colunaNomes.Length
				 && colunaTipos != null && tabelaDados.Rows.Count == colunaTipos.Length)
			{
				_colunaNomes = colunaNomes;
				_colunaTipos = colunaTipos;

				if (colunaTamanhos != null && colunaTamanhos.Length.Equals(colunaTamanhos.Length))
				{
					_colunaTamanhos = colunaTamanhos;
				}
			}
		}

		#endregion

		#region Métodos

		public byte[] Gerar()
		{
			MemoryStream str = new MemoryStream();

			_xmlEscritor = new XmlTextWriter(str, Encoding.UTF8);

			_xmlEscritor.Formatting = Formatting.Indented;

			_xmlEscritor.WriteStartDocument();

			_xmlEscritor.WriteProcessingInstruction(MSO_APPLICATION, PROGID);

			_xmlEscritor.WriteStartElement(TAG_PASTA, VALOR_NAMESPACE_ESTILO);
			_xmlEscritor.WriteAttributeString(TAG_NAMESPACE, PREFIXO_ESTILO, null, VALOR_NAMESPACE_ESTILO);
			_xmlEscritor.WriteAttributeString(TAG_NAMESPACE, PREFIXO_EXCEL, null, VALOR_NAMESPACE_EXCEL);

			EscreverEstilo();

			EscreverTabela();

			_xmlEscritor.WriteEndElement();
			_xmlEscritor.WriteEndDocument();
			_xmlEscritor.Close();

			return str.ToArray();
		}

		private void EscreverEstilo()
		{
			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILOS);

			#region Padrão

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.PADRAO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_NOME, null, Estilos.PADRAO_VALOR_ESTILO_NOME);

			_xmlEscritor.WriteStartElement(Estilos.TAG_ALINHAMENTO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ALINHAMENTO_VERTICAL, null, Estilos.PADRAO_VALOR_ALINHAMENTO_VERTICAL);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FONTE);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FONTE_NOME, null, Estilos.PADRAO_VALOR_FONTE_NOME);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FONTE_FAMILIA, null, Estilos.PADRAO_VALOR_FONTE_FAMILIA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FONTE_TAMANHO, null, Estilos.PADRAO_VALOR_FONTE_TAMANHO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FONTE_COR, null, Estilos.PADRAO_VALOR_FONTE_COR);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Cabeçalho

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.CABECALHO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.CABECALHO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_ALINHAMENTO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ALINHAMENTO_HORIZONTAL, null, Estilos.CABECALHO_VALOR_ALINHAMENTO_HORIZONTAL);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FONTE);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FONTE_TAMANHO, null, Estilos.CABECALHO_VALOR_FONTE_TAMANHO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FONTE_COR, null, Estilos.CABECALHO_VALOR_FONTE_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FONTE_NEGRITO, null, Estilos.CABECALHO_VALOR_FONTE_NEGRITO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.CABECALHO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.CABECALHO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Colunas

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.COLUNA_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.COLUNA_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.COLUNA_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.COLUNA_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.COLUNA_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.COLUNA_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.COLUNA_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.COLUNA_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.COLUNA_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.COLUNA_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.COLUNA_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.COLUNA_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.COLUNA_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.COLUNA_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FONTE);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FONTE_COR, null, Estilos.COLUNA_VALOR_FONTE_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FONTE_NEGRITO, null, Estilos.COLUNA_VALOR_FONTE_NEGRITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FONTE_TAMANHO, null, Estilos.PADRAO_VALOR_FONTE_TAMANHO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.COLUNA_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.COLUNA_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Texto Claro

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.TEXTO_CLARO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.TEXTO_CLARO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.TEXTO_CLARO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.TEXTO_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.TEXTO_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.TEXTO_CLARO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.TEXTO_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.TEXTO_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.TEXTO_CLARO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.TEXTO_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.TEXTO_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.TEXTO_CLARO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.TEXTO_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.TEXTO_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.TEXTO_CLARO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.TEXTO_CLARO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Número Extenso Claro

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FORMATACAO_NUMERO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FORMATACAO_NUMERO_FORMATO, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_FORMATACAO_NUMERO_FORMATO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Número Claro

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.NUMERO_CLARO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.NUMERO_CLARO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_ALINHAMENTO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ALINHAMENTO_HORIZONTAL, null, Estilos.NUMERO_CLARO_VALOR_ALINHAMENTO_HORIZONTAL);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_CLARO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_CLARO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_CLARO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_CLARO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.NUMERO_CLARO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.NUMERO_CLARO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FORMATACAO_NUMERO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FORMATACAO_NUMERO_FORMATO, null, Estilos.NUMERO_CLARO_VALOR_FORMATACAO_NUMERO_FORMATO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Número Claro - Dois Dígitos

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.NUMERO2_CLARO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.NUMERO2_CLARO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_ALINHAMENTO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ALINHAMENTO_HORIZONTAL, null, Estilos.NUMERO2_CLARO_VALOR_ALINHAMENTO_HORIZONTAL);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO2_CLARO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO2_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO2_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO2_CLARO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO2_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO2_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO2_CLARO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO2_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO2_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO2_CLARO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO2_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO2_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.NUMERO2_CLARO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.NUMERO2_CLARO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FORMATACAO_NUMERO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FORMATACAO_NUMERO_FORMATO, null, Estilos.NUMERO2_CLARO_VALOR_FORMATACAO_NUMERO_FORMATO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Número Claro - Quatro Dígitos

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.NUMERO4_CLARO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.NUMERO4_CLARO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_ALINHAMENTO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ALINHAMENTO_HORIZONTAL, null, Estilos.NUMERO4_CLARO_VALOR_ALINHAMENTO_HORIZONTAL);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO4_CLARO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO4_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO4_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO4_CLARO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO4_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO4_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO4_CLARO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO4_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO4_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO4_CLARO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO4_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO4_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.NUMERO4_CLARO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.NUMERO4_CLARO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FORMATACAO_NUMERO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FORMATACAO_NUMERO_FORMATO, null, Estilos.NUMERO4_CLARO_VALOR_FORMATACAO_NUMERO_FORMATO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Data Claro

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.DATA_CLARO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.DATA_CLARO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_CLARO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_CLARO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_CLARO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_CLARO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.DATA_CLARO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.DATA_CLARO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FORMATACAO_NUMERO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FORMATACAO_NUMERO_FORMATO, null, Estilos.DATA_CLARO_VALOR_FORMATACAO_NUMERO_FORMATO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Data Curta Claro

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.DATA_CURTA_CLARO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.DATA_CURTA_CLARO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_CURTA_CLARO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_CURTA_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_CURTA_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_CURTA_CLARO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_CURTA_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_CURTA_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_CURTA_CLARO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_CURTA_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_CURTA_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_CURTA_CLARO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_CURTA_CLARO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_CURTA_CLARO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.DATA_CURTA_CLARO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.DATA_CURTA_CLARO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FORMATACAO_NUMERO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FORMATACAO_NUMERO_FORMATO, null, Estilos.DATA_CURTA_CLARO_VALOR_FORMATACAO_NUMERO_FORMATO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Texto Escuro

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.TEXTO_ESCURO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.TEXTO_ESCURO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.TEXTO_ESCURO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.TEXTO_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.TEXTO_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.TEXTO_ESCURO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.TEXTO_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.TEXTO_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.TEXTO_ESCURO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.TEXTO_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.TEXTO_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.TEXTO_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.TEXTO_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.TEXTO_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.TEXTO_ESCURO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.TEXTO_ESCURO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Npumero Extenso Escuro

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FORMATACAO_NUMERO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FORMATACAO_NUMERO_FORMATO, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_FORMATACAO_NUMERO_FORMATO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Número Escuro

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.NUMERO_ESCURO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.NUMERO_ESCURO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_ALINHAMENTO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ALINHAMENTO_HORIZONTAL, null, Estilos.NUMERO_ESCURO_VALOR_ALINHAMENTO_HORIZONTAL);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_ESCURO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_ESCURO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_ESCURO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.NUMERO_ESCURO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.NUMERO_ESCURO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FORMATACAO_NUMERO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FORMATACAO_NUMERO_FORMATO, null, Estilos.NUMERO_ESCURO_VALOR_FORMATACAO_NUMERO_FORMATO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Número Escuro - Dois Dígitos

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.NUMERO2_ESCURO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.NUMERO2_ESCURO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_ALINHAMENTO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ALINHAMENTO_HORIZONTAL, null, Estilos.NUMERO2_ESCURO_VALOR_ALINHAMENTO_HORIZONTAL);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO2_ESCURO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO2_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO2_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO2_ESCURO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO2_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO2_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO2_ESCURO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO2_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO2_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO2_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO2_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO2_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.NUMERO2_ESCURO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.NUMERO2_ESCURO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FORMATACAO_NUMERO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FORMATACAO_NUMERO_FORMATO, null, Estilos.NUMERO2_ESCURO_VALOR_FORMATACAO_NUMERO_FORMATO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Número Escuro - Quatro Dígitos

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.NUMERO4_ESCURO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.NUMERO4_ESCURO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_ALINHAMENTO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ALINHAMENTO_HORIZONTAL, null, Estilos.NUMERO4_ESCURO_VALOR_ALINHAMENTO_HORIZONTAL);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO4_ESCURO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO4_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO4_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO4_ESCURO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO4_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO4_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO4_ESCURO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO4_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO4_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.NUMERO4_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.NUMERO4_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.NUMERO4_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.NUMERO4_ESCURO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.NUMERO4_ESCURO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FORMATACAO_NUMERO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FORMATACAO_NUMERO_FORMATO, null, Estilos.NUMERO4_ESCURO_VALOR_FORMATACAO_NUMERO_FORMATO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Data Escuro

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.DATA_ESCURO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.DATA_ESCURO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_ESCURO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_ESCURO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_ESCURO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.DATA_ESCURO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.DATA_ESCURO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FORMATACAO_NUMERO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FORMATACAO_NUMERO_FORMATO, null, Estilos.DATA_ESCURO_VALOR_FORMATACAO_NUMERO_FORMATO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			#region Data Curta Escuro

			_xmlEscritor.WriteStartElement(Estilos.TAG_ESTILO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_ID, null, Estilos.DATA_CURTA_ESCURO_VALOR_ESTILO_ID);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_ESTILO_PAI, null, Estilos.DATA_CURTA_ESCURO_VALOR_ESTILO_PAI);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDAS);

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_CURTA_ESCURO_VALOR_BORDA_POSICAO_BAIXO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_CURTA_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_CURTA_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_CURTA_ESCURO_VALOR_BORDA_POSICAO_TOPO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_CURTA_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_CURTA_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_CURTA_ESCURO_VALOR_BORDA_POSICAO_DIREITO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_CURTA_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_CURTA_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_BORDA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_POSICAO, null, Estilos.DATA_CURTA_ESCURO_VALOR_BORDA_POSICAO_ESQUERDO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_ESTILO_LINHA, null, Estilos.DATA_CURTA_ESCURO_VALOR_BORDA_ESTILO_LINHA);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_BORDA_PESO, null, Estilos.DATA_CURTA_ESCURO_VALOR_BORDA_PESO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_INTERIOR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_COR, null, Estilos.DATA_CURTA_ESCURO_VALOR_INTERIOR_COR);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_INTERIOR_MODELO, null, Estilos.DATA_CURTA_ESCURO_VALOR_INTERIOR_MODELO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Estilos.TAG_FORMATACAO_NUMERO);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Estilos.TAG_FORMATACAO_NUMERO_FORMATO, null, Estilos.DATA_CURTA_ESCURO_VALOR_FORMATACAO_NUMERO_FORMATO);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			#endregion

			_xmlEscritor.WriteEndElement();
		}

		private void EscreverTabela()
		{
			_xmlEscritor.WriteStartElement(Planilha.TAG);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Planilha.TAG_NOME, null, Planilha.VALOR_NOME);

			_xmlEscritor.WriteStartElement(Planilha.TAG_NOMES);

			_xmlEscritor.WriteStartElement(Planilha.TAG_NOMES_ALCANCE);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Planilha.TAG_NOMES_ALCANCE_NOME, null, Planilha.VALOR_NOMES_ALCANCE_NOME);
			_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Planilha.TAG_NOMES_ALCANCE_INVISIVEL, null, Planilha.VALOR_NOMES_ALCANCE_INVISIVEL);

			if (_colunaNomes != null && _colunaNomes.Length > 0)
			{
				_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Planilha.TAG_NOMES_ALCANCE_REFERENCIA, null, Planilha.VALOR_NOMES_ALCANCE_REFERENCIA + _colunaNomes.Length);
			}

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Tabela.TAG);

			if (_colunaNomes != null && _colunaNomes.Length > 0)
			{
				_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_TABELA_COLUNAS_PREENCHIDAS, null, Tabela.VALOR_TABELA_COLUNAS_PREENCHIDAS);
				_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_TABELA_LINHAS_PREENCHIDAS, null, Tabela.VALOR_TABELA_LINHAS_PREENCHIDAS);
				_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_TABELA_COLUNAS_EXPANDIDAS, null, _colunaNomes.Length.ToString());
				_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_TABELA_LINHAS_EXPANDIDAS, null, (_tabelaDados.Rows.Count + 2).ToString());
				_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_TABELA_LINHA_TAMANHO_PADRAO, null, Tabela.VALOR_TABELA_LINHA_TAMANHO_PADRAO);

				for (int i = 0; i < _colunaNomes.Length; ++i)
				{
					_xmlEscritor.WriteStartElement(Tabela.TAG_COLUNA);

					_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_COLUNA_TAMANHO_AUTOMATICO, null, Tabela.VALOR_COLUNA_TAMANHO_AUTOMATICO);

					if (_colunaTamanhos != null && _colunaTamanhos.Length - 1 >= i && _colunaTamanhos[i] > 0)
					{
						_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_COLUNA_TAMANHO, null, _colunaNomes[i].ToString());
					}
					else
					{
						_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_COLUNA_TAMANHO, null, CalcularTamanhoCelula(_colunaNomes[i]));
					}

					_xmlEscritor.WriteEndElement();
				}

				#region Cabeçalho

				_xmlEscritor.WriteStartElement(Tabela.TAG_LINHA);
				_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_LINHA_ALTURA, null, Tabela.VALOR_LINHA_ALTURA_CABECALHO);

				_xmlEscritor.WriteStartElement(Tabela.TAG_CELULA);
				_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_MESCLA, null, (_colunaNomes.Length - 1).ToString());
				_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.CABECALHO_VALOR_ESTILO_ID);

				_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);
				_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.String.ToString());
				_xmlEscritor.WriteValue(_tituloCabecalho);

				_xmlEscritor.WriteEndElement();
				_xmlEscritor.WriteEndElement();
				_xmlEscritor.WriteEndElement();

				#endregion

				#region Colunas

				_xmlEscritor.WriteStartElement(Tabela.TAG_LINHA);

				for (int i = 0; i < _colunaNomes.Length; ++i)
				{
					_xmlEscritor.WriteStartElement(Tabela.TAG_CELULA);
					_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.COLUNA_VALOR_ESTILO_ID);

					_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);
					_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.String.ToString());
					_xmlEscritor.WriteValue(_colunaNomes[i]);

					_xmlEscritor.WriteEndElement();
					_xmlEscritor.WriteEndElement();
				}

				_xmlEscritor.WriteEndElement();

				#endregion

				#region Dados

				for (int i = 0; i < _tabelaDados.Rows.Count; ++i)
				{
					_xmlEscritor.WriteStartElement(Tabela.TAG_LINHA);

					for (int j = 0; j < _tabelaDados.Columns.Count; ++j)
					{
						_xmlEscritor.WriteStartElement(Tabela.TAG_CELULA);

						if (i % 2 == 0)
						{
							#region Claro

							switch (_colunaTipos[j])
							{
								case EDadoTipo.String:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.TEXTO_CLARO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.String.ToString());
									break;

								case EDadoTipo.Number:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.NUMERO_CLARO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.Number.ToString());
									break;

								case EDadoTipo.Number2:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.NUMERO2_CLARO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.Number.ToString());
									break;

								case EDadoTipo.Number4:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.NUMERO4_CLARO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.Number.ToString());
									break;

								case EDadoTipo.NumberLarge:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.NUMERO_EXTENSO_CLARO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.String.ToString());

									_tabelaDados.Rows[i][_colunaNomes[j]] += " ";
									break;

								case EDadoTipo.DateTime:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.DATA_CLARO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);

									DateTime data = DateTime.Now;

									if (DateTime.TryParse(_tabelaDados.Rows[i][_colunaNomes[j]].ToString(), out data) && data.Year >= 1900)
									{
										_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.DateTime.ToString());
										_tabelaDados.Rows[i][_colunaNomes[j]] = DataFormatacao(_tabelaDados.Rows[i][_colunaNomes[j]].ToString());
									}
									else
									{
										_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.String.ToString());
									}
									break;

								case EDadoTipo.DateTimeShort:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.DATA_CURTA_CLARO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);

									DateTime dataCurta = DateTime.Now;

									if (DateTime.TryParse(_tabelaDados.Rows[i][_colunaNomes[j]].ToString(), out data) && data.Year >= 1900)
									{
										_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.DateTime.ToString());
										_tabelaDados.Rows[i][_colunaNomes[j]] = DataFormatacao(_tabelaDados.Rows[i][_colunaNomes[j]].ToString());
									}
									else
									{
										_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.String.ToString());
									}
									break;
							}

							#endregion
						}
						else
						{
							#region Escuro

							switch (_colunaTipos[j])
							{
								case EDadoTipo.String:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.TEXTO_ESCURO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.String.ToString());
									break;

								case EDadoTipo.Number:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.NUMERO_ESCURO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.Number.ToString());
									break;

								case EDadoTipo.Number2:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.NUMERO2_ESCURO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.Number.ToString());
									break;

								case EDadoTipo.Number4:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.NUMERO4_ESCURO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.Number.ToString());
									break;

								case EDadoTipo.NumberLarge:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.NUMERO_EXTENSO_ESCURO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.String.ToString());

									_tabelaDados.Rows[i][_colunaNomes[j]] += " ";
									break;

								case EDadoTipo.DateTime:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.DATA_ESCURO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);

									DateTime data = DateTime.Now;

									if (DateTime.TryParse(_tabelaDados.Rows[i][_colunaNomes[j]].ToString(), out data) && data.Year >= 1900)
									{
										_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.DateTime.ToString());
										_tabelaDados.Rows[i][_colunaNomes[j]] = DataFormatacao(_tabelaDados.Rows[i][_colunaNomes[j]].ToString());
									}
									else
									{
										_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.String.ToString());
									}
									break;

								case EDadoTipo.DateTimeShort:
									_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_CELULA_ESTILO, null, Estilos.DATA_CURTA_ESCURO_VALOR_ESTILO_ID);

									_xmlEscritor.WriteStartElement(Tabela.TAG_DADO);

									DateTime dataCurta = DateTime.Now;

									if (DateTime.TryParse(_tabelaDados.Rows[i][_colunaNomes[j]].ToString(), out data) && data.Year >= 1900)
									{
										_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.DateTime.ToString());
										_tabelaDados.Rows[i][_colunaNomes[j]] = DataFormatacao(_tabelaDados.Rows[i][_colunaNomes[j]].ToString());
									}
									else
									{
										_xmlEscritor.WriteAttributeString(PREFIXO_ESTILO, Tabela.TAG_DADO_TIPO, null, EDadoTipo.String.ToString());
									}
									break;
							}

							#endregion
						}

						if (Convert.IsDBNull(_tabelaDados.Rows[i][_colunaNomes[j]]) || _tabelaDados.Rows[i][_colunaNomes[j]].ToString() == null)
						{
							_xmlEscritor.WriteValue(string.Empty);
						}
						else
						{
							_xmlEscritor.WriteValue(_tabelaDados.Rows[i][_colunaNomes[j]]);
						}

						_xmlEscritor.WriteEndElement();
						_xmlEscritor.WriteEndElement();
					}

					_xmlEscritor.WriteEndElement();
				}

				#endregion
			}

			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteStartElement(Planilha.TAG_FILTRO_AUTOMATICO);

			if (_colunaNomes != null && _colunaNomes.Length > 0)
			{
				_xmlEscritor.WriteAttributeString(PREFIXO_EXCEL, Planilha.TAG_FILTRO_AUTOMATICO_ALCANCE, null, Planilha.VALOR_FILTRO_AUTOMATICO_ALCANCE + _colunaNomes.Length);
			}

			_xmlEscritor.WriteAttributeString(TAG_NAMESPACE, VALOR_NAMESPACE_EXCEL);
			_xmlEscritor.WriteEndElement();

			_xmlEscritor.WriteEndElement();
		}

		private string CalcularTamanhoCelula(string coluna)
		{
			Font fonteDesenho = new Font(Estilos.PADRAO_VALOR_FONTE_NOME, float.Parse(Estilos.PADRAO_VALOR_FONTE_TAMANHO));
			Graphics grafico = Graphics.FromImage(new Bitmap(1, 1));
			double fonteTamanhoTotal = 0.0f;

			foreach (DataRow linha in _tabelaDados.Rows)
			{
				double fonteTamanho = (double)grafico.MeasureString(linha[coluna].ToString(), fonteDesenho).Width;

				if (fonteTamanhoTotal == 0f || fonteTamanho > fonteTamanhoTotal)
				{
					fonteTamanhoTotal = fonteTamanho;
				}

				if (fonteTamanhoTotal > 400)
				{
					fonteTamanhoTotal = 400;
					break;
				}
			}

			double fonteTamanhoColuna = (double)grafico.MeasureString(coluna, fonteDesenho).Width;

			if (fonteTamanhoTotal < 400 && fonteTamanhoTotal < fonteTamanhoColuna)
			{
				fonteTamanhoTotal = fonteTamanhoColuna;
			}

			grafico.Dispose();

			return fonteTamanhoTotal.ToString("F", CultureInfo.InvariantCulture);
		}

		private string DataFormatacao(string data)
		{
			DateTime dataConvertida = DateTime.MinValue;

			DateTime.TryParse(data, out dataConvertida);

			return dataConvertida.ToString("s");
		}

		#endregion
	}
}
