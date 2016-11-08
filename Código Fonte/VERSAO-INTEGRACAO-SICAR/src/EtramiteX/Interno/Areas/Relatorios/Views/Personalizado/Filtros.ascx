<%@ Import Namespace="Tecnomapas.Blocos.RelatorioPersonalizado.Entities" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersonalizadoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<h5>Filtro</h5>

<div class="block borderB padding0">
	<div class="block">
		<div class="coluna20">
			<p>
				<label for="Campo">Campo</label>
				<select name="Campo" class="text setarFoco ddlCampo">
					<option value="<%: ViewModelHelper.Json(new Campo()) %>">*** Selecione ***</option>
					<% foreach (var item in Model.ConfiguracaoRelatorio.FonteDados.CamposFiltro) { %>
					<option value="<%: ViewModelHelper.Json(item) %>"><%: item.DimensaoNome + " - " + item.Alias %></option>
					<% } %>
				</select>
			</p>
		</div>
		<div class="coluna15">
			<p>
				<label for="Operador">Operador</label>
				<select name="Operador" class="text ddlOperador"></select>
			</p>
		</div>
		<div class="coluna30">
			<p>
				<label for="Filtro">Filtro</label>
				<input type="text" class="text txtFiltro campoFiltro" name="Filtro" maxlength="30" />
				<select name="Filtro" class="text ddlFiltro campoFiltro hide"></select>
			</p>
		</div>
		<div class="coluna20">
			<p class="paddingT20">
				<label class="labelCheckBox"><input type="checkbox" class="cbDefinirExecucao" /><span>Definir na Execução</span></label>
			</p>
		</div>

		<div class="coluna11 ultima">
			<button class="btnAdicionar floatRight inlineBotao botaoAdicionarIcone" title="Adicionar">Adicionar</button>
		</div>
	</div>

	<div id="accordion" class="divFiltroCheck coluna100 hide">
		<h3><a href="#">Filtro</a> <span class="invertkAll">Inverter Marcação</span><span class="uncheckAll">Desmarcar Todos</span><span class="checkAll">Marcar Todos</span></h3>
		<div class="divChecks block coluna100"></div>
	</div>
</div>

<h5 class="margemDTop">Construtor de Filtro Multiplo</h5>
<div class="block">
	<div class="coluna60">
		<ul class="dragButtons">
			<li class="adicional" title="E"><input type="hidden" class="termo" value="<%: ViewModelHelper.Json(new { Tipo = (int)eTipoTermo.OperadorE }) %>" /> E</li>
			<li class="adicional" title="OU"><input type="hidden" class="termo" value="<%: ViewModelHelper.Json(new { Tipo = (int)eTipoTermo.OperadorOu }) %>" />OU</li>
			<li class="abreParenteses" title="Abre Parênteses"><input type="hidden" class="termo" value="<%: ViewModelHelper.Json(new { Tipo = (int)eTipoTermo.AbreParenteses }) %>" />(</li>
			<li class="fechaParenteses" title="Fecha Parênteses"><input type="hidden" class="termo" value="<%: ViewModelHelper.Json(new { Tipo = (int)eTipoTermo.FechaParenteses }) %>" />)</li>
		</ul>
		<span class="dica quiet margemEsq">Arraste os operadores para montar o filtro.</span>
	</div>
</div>
<div class="block">
	<ul class="areaFiltroMultiplo">
		<% foreach (var item in Model.ConfiguracaoRelatorio.Termos) { %>
		<% switch ((eTipoTermo)item.Tipo) { %>
			<% case eTipoTermo.AbreParenteses: %>
				<li class="abreParenteses" title="Abre Parênteses"><input type="hidden" class="termo" value="<%: ViewModelHelper.Json(item) %>" />(</li>
				<% break; %>

			<% case eTipoTermo.FechaParenteses: %>
				<li class="fechaParenteses" title="Fecha Parênteses"><input type="hidden" class="termo" value="<%: ViewModelHelper.Json(item) %>" />)</li>
				<% break; %>

			<% case eTipoTermo.Filtro: %>
				<% string operador = Model.OperadoresLst.SingleOrDefault(x => x.Value == item.Operador.ToString()).Text; %>
				<li class="filtro">
					<input type="hidden" class="termo" value="<%: ViewModelHelper.Json(item) %>" />
					<span><%: item.Campo.Alias %></span>
					<em title="<%= operador %>"><%= operador %></em>
					<% string filtro = item.Valor;
					if (item.DefinirExecucao)
					{
						filtro = "Definido na Execução";
					}
					else if (item.Campo.PossuiListaDeValores && !string.IsNullOrEmpty(filtro))
					{
						if (item.Campo.TipoDadosEnum == eTipoDados.Bitand)
						{
							int valor = Convert.ToInt32(filtro);
							filtro = string.Empty;
							
							foreach (var itemLista in item.Campo.Lista)
							{
								if ((valor & Convert.ToInt32(itemLista.Codigo)) > 0)
								{
									filtro += itemLista.Texto + '/';
								}
							}

							filtro = filtro.Substring(0, filtro.Length - 1);
						}
						else
						{
							filtro = item.Campo.Lista.SingleOrDefault(x => x.Id == filtro).Texto;
						}
					}%>
					<span><%: filtro %></span>
				</li>
				<% break; %>

			<% case eTipoTermo.OperadorE: %>
				<li class="adicional" title="E"><input type="hidden" class="termo" value="<%: ViewModelHelper.Json(item) %>" /> E</li>
				<% break; %>

			<% case eTipoTermo.OperadorOu: %>
				<li class="adicional" title="OU"><input type="hidden" class="termo" value="<%: ViewModelHelper.Json(item) %>" />OU</li>
				<% break; %>
		<% } %>
		<% } %>
	</ul>
	<button class="dell"><%--<span>Deletar Item</span>--%></button>
</div>