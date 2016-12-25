<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<GerenciarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Gerenciar</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/OrgaosParceirosConveniados/orgaoParceiroGerenciar.js") %>"></script>
	<script>
		$(function () {
			OrgaoParceiroGerenciar.load($('#central'), {
				urls: {
					buscar: '<%=Url.Action("GerenciarFiltrar", "OrgaosParceirosConveniados")%>',
					visualizar: '<%= Url.Action("Visualizar", "Credenciado") %>',
					gerarChave: '<%= Url.Action("GerarChave", "OrgaosParceirosConveniados") %>',
					bloquear: '<%= Url.Action("Bloquear", "OrgaosParceirosConveniados") %>',
					desbloquear: '<%= Url.Action("Desbloquear", "OrgaosParceirosConveniados") %>'
				},
				Mensagens: <%=Model.Mensagens%>
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="central">
	<h1 class="titTela">Gerenciar Credenciados - Parceiros/ Conveniados</h1>
	<br />
	<%=Html.Hidden("idOrgao", Model.IdOrgao, new { @class="hdnIdOrgao"}) %>
	<div class="block filtroExpansivo">
		<span class="titFiltroSimples">Filtro</span>
		<div class="filtroCorpo block">
			<div class="block fixado">
				<div class="coluna20">
					<label>Sigla</label>
					<%=Html.TextBox("Sigla", Model.Sigla, new{ @class="txtFiltroSigla text disabled", @disabled="disabled" }) %>
				</div>
				<div class="ultima block">
					<label>Nome do órgão</label>
					<%=Html.TextBox("Nome_Orgao", Model.NomeOrgao, new{ @class="txtFiltroNomeOrgao text disabled", @disabled="disabled" }) %>
				</div>
			</div>
			<div class="block">
				<div class="coluna40">
					<label>Unidade</label>
					<%=Html.DropDownList("Unidades", Model.Unidades, new {@class="ddlUnidades text"}) %>
				</div>
				<div class="coluna10">
					<button class="inlineBotao btnBuscar">Buscar</button>
				</div>
			</div>
		</div>
	</div>

	<div class="block">
		<fieldset class="<%=Model.CredenciadosAguardandoAtivacao.Credenciados.Count < 1? "hide " :"" %> fsCredenciadosAguardandoAtivacao fieldExpansivo box">
			<legend class="fAberto">Credenciados Parceiros/ Conveniados aguardando ativação</legend>
			<br />
			<div class="expandirRetrair">
				<div class="divAguardando coluna99 prepend1"></div>
				<div class="coluna15">
					<button class="inlineBotao btnGerarChave">Gerar Chave</button>
				</div>
			</div>
		</fieldset>
	</div>

	<div class="block">
		<fieldset class="<%=Model.CredenciadosAtivos.Credenciados.Count < 1? "hide " :"" %> fsCredenciadosAtivos fieldExpansivo box ">
			<legend class="fAberto">Credenciados Parceiros/ Conveniados ativos</legend>
			<br />
			<div class="expandirRetrair">
				<div class="divAtivos coluna99 prepend1"></div>

				<div class="coluna15 prepend1">
					<button class="inlineBotao btnBloquear">Bloquear</button>
				</div>
			</div>
		</fieldset>

	</div>

	<div class="block">
		<fieldset class="<%=Model.CredenciadosBloqueados.Credenciados.Count < 1? "hide " :"" %> fsCredenciadosBloqueados fieldExpansivo box">
			<legend class="fAberto">Credenciados Parceiros/ Conveniados bloqueados</legend>
			<br />
			<div class="expandirRetrair">
				<div class="divBloqueados coluna99 prepend1"></div>
				<div class="coluna15 prepend1">
					<button class="inlineBotao btnDesbloquear">Desbloquear</button>
				</div>
			</div>
		</fieldset>
	</div>
	<div class="block box">
		<span class="cancelarCaixa"><a class="linkCancelar" href='<%=Url.Action("Index", "OrgaosParceirosConveniados") %>'>Cancelar</a></span>
	</div>
</div>

</asp:Content>


