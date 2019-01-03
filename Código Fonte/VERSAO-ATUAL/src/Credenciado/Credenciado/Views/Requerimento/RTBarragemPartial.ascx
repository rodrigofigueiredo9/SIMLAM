<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Requerimento>" %>

<h1 class="titTela">Responsável Técnico</h1>
<br />

<%--<input type="hidden" class="hdnEmissaoId" value="<%= Model.Id %>" />
<input type="hidden" class="hdnEmissaoNumero" value="<%= Model.Numero %>" />
<input type="hidden" class="hdnEmissaoTipoNumero" value="<%= Model.TipoNumero.GetValueOrDefault() %>" />--%>

<div class="block box">
	<div class="block">
		<label>Além do cadastro da declaração de dispensa, você é o responsável técnico pela elaboração de:</label>
		<br />
		<label>
				<%=Html.RadioButton("Requerimento.FuncaoResponsavel", 1, false, ViewModelHelper.SetaDisabled(false, new { @class="rbFuncaoRT rbProjetoLaudo"}))%>
				Projeto técnico/laudo de barragem construída
		</label><br />
		<label>
				<%=Html.RadioButton("Requerimento.FuncaoResponsavel", 2, false, ViewModelHelper.SetaDisabled(false, new { @class="rbFuncaoRT rbEstudo"}))%>
				Estudo ambiental
		</label><br />
		<label>
				<%=Html.RadioButton("Requerimento.FuncaoResponsavel", 3, false, ViewModelHelper.SetaDisabled(false, new { @class="rbFuncaoRT Ambos"}))%>
				Ambos
		</label>
	</div>

	<%--<div class="coluna15">
		<label for="DataAtivacao">Data de ativação *</label>
		<%=Html.TextBox("DataAtivacao", Model.DataAtivacao.DataTexto, ViewModelHelper.SetaDisabled(Model.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital, new { @class="text maskData txtDataAtivacao"}))%>
	</div>--%>
</div>