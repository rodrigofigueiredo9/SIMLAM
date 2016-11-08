using System;

namespace Tecnomapas.Blocos.Arquivo
{
	public class ArquivoSerializavel
	{
		public int? Id { get; set; }
		public int Raiz { get; set; }
		public String Nome { get; set; }
		public String Extensao { get; set; }
		public String Caminho { get; set; }
		public String Diretorio { get; set; }
		public String TemporarioNome { get; set; }
		public String TemporarioPathNome { get; set; }
		public String ContentType { get; set; }
		public Int32 ContentLength { get; set; }
		public String Tid { get; set; }
		public Int32 Apagar { get; set; }

		public static ArquivoSerializavel Load(Arquivo arquivo)
		{
			ArquivoSerializavel arqSer = new ArquivoSerializavel();
			arqSer.Id = arquivo.Id;
			arqSer.Raiz = arquivo.Raiz;
			arqSer.Nome = arquivo.Nome;
			arqSer.Extensao = arquivo.Extensao;
			arqSer.Caminho = arquivo.Caminho;
			arqSer.Diretorio = arquivo.Diretorio;
			arqSer.TemporarioNome = arquivo.TemporarioNome;
			arqSer.TemporarioPathNome = arquivo.TemporarioPathNome;
			arqSer.ContentType = arquivo.ContentType;
			arqSer.ContentLength = arquivo.ContentLength;
			arqSer.Tid = arquivo.Tid;
			arqSer.Apagar = arquivo.Apagar;
			return arqSer;
		}

		public Arquivo ToArquivo()
		{
			Arquivo arquivo = new Arquivo();
			arquivo.Id = Id;
			arquivo.Raiz = Raiz;
			arquivo.Nome = Nome;
			arquivo.Extensao = Extensao;
			arquivo.Caminho = Caminho;
			arquivo.Diretorio = Diretorio;
			arquivo.TemporarioNome = TemporarioNome;
			arquivo.TemporarioPathNome = TemporarioPathNome;
			arquivo.ContentType = ContentType;
			arquivo.ContentLength = ContentLength;
			arquivo.Tid = Tid;
			arquivo.Apagar = Apagar;

			return arquivo;
		}
	}
}
