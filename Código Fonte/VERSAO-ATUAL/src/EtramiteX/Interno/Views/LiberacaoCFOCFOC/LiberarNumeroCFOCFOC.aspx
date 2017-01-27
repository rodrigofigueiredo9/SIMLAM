<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMLiberacaoCFOCFOC" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<LiberarNumeroCFOCFOCVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Habilitar Emissão de CFO e CFOC</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Pessoa/Pessoa.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/Pessoa/associar.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>
	<script src="<%= Url.Content("~/Scripts/LiberacaoCFOCFOC/liberarNumeroCFOCFOC.js") %>"></script>
	<script>
		$(function () {
			LiberarNumeroCFOCFOC.load($('#central'),
			{
				urls:
				{
					verificarCPF: '<%=Url.Action("VerificarCPF", "LiberacaoCFOCFOC")%>',
					visualizarPessoa: '<%=Url.Action("Visualizar", "Credenciado")%>',
					salvar: '<%=Url.Action("SalvarLiberacao", "LiberacaoCFOCFOC")%>'
				}
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">

		<% if (Model.isVisualizar) { %>
		<h1 class="titTela">Visualizar Liberações de Números de CFO e CFOC</h1>
		<% } else { %>
		<h1 class="titTela">Liberar Número de CFO e CFOC</h1>
		<% } %>
		<br />
		<fieldset class="block box">
			<legend>Responsáveis Técnicos</legend>
			<div class="block">
				<div class="coluna15">
					<label for="Pessoa_Fisica_Cpf">CPF *</label>
					<%= Html.TextBox("Pessoa.Fisica.Cpf", Model.Liberacao.CPF, ViewModelHelper.SetaDisabled(Model.isVisualizar, new { @class = "text setarFoco maskCpf txtCpf", @maxlength = "14" }))%>
					<%= Html.Hidden("Credenciado.Id", 0, new { @class = "hdnCredenciadoId" })%>
				</div>
				<%if (!Model.isVisualizar) { %>
				<div class="coluna20 prepend2">
					<button type="button" class="inlineBotao esquerda btnVerificarCpf">Verificar</button>
					<button type="button" class="inlineBotao esquerda icone visualizar btnVisualizarPessoa hide mostrar"></button>
				</div>
				<%} %>
			</div>
			<div class="block mostrar <%=Model.isVisualizar? "": "hide" %>">
				<div class="coluna70">
					<label for="Liberacao_Nome">Nome *</label>
					<%= Html.TextBox("Liberacao.Nome", Model.Liberacao.Nome, ViewModelHelper.SetaDisabled(Model.isVisualizar, new { @class = "text txtNome", @maxlength = "80" }))%>
					<%= Html.Hidden("Pessoa.Fisica.Id", 0, new { @class = "hdnPessoaId" })%>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box mostrar <%=Model.isVisualizar? "": "hide" %>">
			<legend>Nº Bloco *</legend>

			<div class="block">
				<div class="coluna40">
					<label>
						<%=Html.CheckBox("Liberar_CFO", Model.Liberacao.LiberarBlocoCFO, ViewModelHelper.SetaDisabled(Model.isVisualizar, new {@class="cbLiberarBlocoCFO"})) %>
						Liberar nº bloco CFO
					</label>
				</div>
			</div>
			<div class="block <%=Model.isVisualizar && Model.Liberacao.LiberarBlocoCFO? "": "hide" %>">
				<div class="coluna30">
					<label for="Liberacao_NumeroInicialBlocoCFO">Nº inicial do bloco CFO *</label>
					<%= Html.TextBox("Liberacao.NumeroInicialBlocoCFO", Model.Liberacao.NumeroInicialCFO>0? Model.Liberacao.NumeroInicialCFO.ToString(): "", ViewModelHelper.SetaDisabled(Model.isVisualizar, new { @class = "text maskNumInt txtNumeroInicialBlocoCFO", @maxlength = "8" }))%>
				</div>
				<div class="coluna30 prepend2">
					<label for="Liberacao_NumeroFinalBlocoCFO">Nº final do bloco CFO *</label>
					<%= Html.TextBox("Liberacao.NumeroFinalBlocoCFO", Model.Liberacao.NumeroFinalCFO>0?Model.Liberacao.NumeroFinalCFO.ToString():"", ViewModelHelper.SetaDisabled(Model.isVisualizar, new { @class = "text maskNumInt txtNumeroFinalBlocoCFO", @maxlength = "8" }))%>
				</div>
			</div>

			<div class="block">
				<div class="coluna20">
					<label>
						<%=Html.CheckBox("Liberar_CFOC", Model.Liberacao.LiberarBlocoCFOC, ViewModelHelper.SetaDisabled(Model.isVisualizar, new {@class="cbLiberarBlocoCFOC"})) %>
						Liberar nº bloco CFOC
					</label>
				</div>
			</div>
			<div class="block <%=Model.isVisualizar && Model.Liberacao.LiberarBlocoCFOC? "": "hide" %>">
				<div class="coluna30">
					<label for="Liberacao_NumeroInicialBlocoCFOC">Nº inicial do bloco CFOC *</label>
					<%= Html.TextBox("Liberacao.NumeroInicialBlocoCFOC", Model.Liberacao.NumeroInicialCFOC > 0? Model.Liberacao.NumeroInicialCFOC.ToString(): "", ViewModelHelper.SetaDisabled(Model.isVisualizar, new { @class = "text maskNumInt txtNumeroInicialBlocoCFOC", @maxlength = "8" }))%>
				</div>
				<div class="coluna30 prepend2">
					<label for="Liberacao_NumeroFinalBlocoCFOC">Nº final do bloco CFOC *</label>
					<%= Html.TextBox("Liberacao.NumeroFinalBlocoCFOC", Model.Liberacao.NumeroFinalCFOC > 0? Model.Liberacao.NumeroFinalCFOC.ToString(): "", ViewModelHelper.SetaDisabled(Model.isVisualizar, new { @class = "text maskNumInt txtNumeroFinalBlocoCFOC", @maxlength = "8" }))%>
				</div>
			</div>
		</fieldset>

		<fieldset class="block box mostrar <%=Model.isVisualizar ? "": "hide" %>">
			<legend>Nº Digital</legend>

			<div class="block">
				<div class="coluna40">
					<label>
						<%=Html.CheckBox("Liberar_Digital_CFO", Model.Liberacao.LiberarDigitalCFO, ViewModelHelper.SetaDisabled(Model.isVisualizar, new {@class="cbLiberarNumeroDigitalCFO"})) %>
						Liberar nº digital CFO
					</label>
				</div>
			</div>
			<div class="block <%=Model.isVisualizar && Model.Liberacao.LiberarDigitalCFO? "": "hide" %>">
				<div class="coluna30">
					<label for="Liberacao_QuantidadeNumeroCFO">Quantidade de Nº CFO *</label>
					<%= Html.DropDownList("Liberacao.QuantidadeNumeroCFO", Model.LstQuantidadeNumeroDigitalCFO, ViewModelHelper.SetaDisabled(Model.isVisualizar, new { @class = "text ddlQtdNumeroDigitalCFO" }))%>
				</div>
			</div>

			<div class="block">
				<div class="coluna40">
					<label>
						<%=Html.CheckBox("Liberar_Digital_CFOC", Model.Liberacao.LiberarDigitalCFOC, ViewModelHelper.SetaDisabled(Model.isVisualizar, new {@class="cbLiberarNumeroDigitalCFOC"})) %>
						Liberar nº digital CFOC
					</label>
				</div>
			</div>
			<div class="block <%=Model.isVisualizar && Model.Liberacao.LiberarDigitalCFOC? "": "hide" %>">
				<div class="coluna30">
					<label for="Liberacao_QuantidadeNumeroDigitalCFOC">Quantidade de Nº CFOC *</label>
					<%= Html.DropDownList("Liberacao.QuantidadeNumeroDigitalCFOC", Model.LstQuantidadeNumeroDigitalCFOC, ViewModelHelper.SetaDisabled(Model.isVisualizar, new { @class = "text ddlQtdNumeroDigitalCFOC" }))%>
				</div>
			</div>
		</fieldset>

		<div class="block box botoesSalvarCancelar">
			<div class="block">
				<%if (!Model.isVisualizar) { %><input class="btnSalvar floatLeft" type="button" value="Salvar" /><%} %>

				<span class="cancelarCaixa">
					<%if (!Model.isVisualizar) { %><span class="btnModalOu">ou</span><%} %>
					<a class="linkCancelar" title="Cancelar" href='<%=Url.Action("Index", "LiberacaoCFOCFOC") %>'>Cancelar</a>
				</span>
			</div>
		</div>
	</div>
</asp:Content>