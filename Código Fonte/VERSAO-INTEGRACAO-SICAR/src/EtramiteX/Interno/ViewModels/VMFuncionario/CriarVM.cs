using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFuncionario
{
	public class CriarVM
	{
		public Funcionario Funcionario { get; set; }
		public Boolean CpfValido { get; set; }
		public string TextoPermissoes { get; set; }

		public List<SelectListItem> Cargos { get; private set; }
		public List<SelectListItem> Setores { get; private set; }
		public List<PapeisVME> Papeis { get; set; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@CargoObrigatorio = Mensagem.Funcionario.CargoObrigatorio,
					@CargoDuplicado = Mensagem.Funcionario.CargoDuplicado,
					@SetoresObrigatorio = Mensagem.Funcionario.SetoresObrigatorio,
                    @ArquivoNaoImagem = Mensagem.Funcionario.ArquivoNaoImagem,
					@SetorDuplicado = Mensagem.Funcionario.SetorDuplicado
                    
				});
			}
		}

		public CriarVM() { }

		public CriarVM(List<Cargo> cargos, List<Setor> setores)
		{
			Funcionario = new Funcionario();
			Papeis = new List<PapeisVME>();

			Cargos = ViewModelHelper.CriarSelectList(cargos, true);
			Setores = ViewModelHelper.CriarSelectList(setores, true);
		}


        public String ObterJSon(Object objeto)
        {
            return ViewModelHelper.JsSerializer.Serialize(objeto);
        }
	}
}