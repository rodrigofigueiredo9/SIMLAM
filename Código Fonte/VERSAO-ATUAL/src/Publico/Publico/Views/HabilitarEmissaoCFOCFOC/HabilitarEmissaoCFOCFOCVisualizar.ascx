<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloCore" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.Model.ModuloHabilitarEmissaoCFOCFOC" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMHabilitarEmissaoCFOCFOC" %>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<HabilitarEmissaoCFOCFOCVM>" %>

<% if (Model.IsAjaxRequest)
   { %>
<script type="text/javascript">
</script>
<% } %>

<div class="modalVisualizarHabilitarEmissaoCFOCFOC">

	<%= Html.Hidden("HabilitarEmissao.Id", Model.HabilitarEmissao.Id, new { @class = "hdnHabilitarId" })%>
	<%= Html.Hidden("HabilitarEmissao.Tid", Model.HabilitarEmissao.Tid, new { @class = "hdnHabilitarTid" })%>
    <%= Html.Hidden("EstadoDefault", Model.HabilitarEmissao.UF, new { @class = "hdnEstadoDefault" })%>
	<%= Html.Hidden("EstadoDefaultSigla", Model.HabilitarEmissao.UFTexto, new { @class = "hdnEstadoDefaultSigla" })%>
	<%= Html.Hidden("hdnResponsavelId", Model.HabilitarEmissao.Responsavel.Id, new { @class = "hdnResponsavelId" })%>

	<h1 class="titTela">Habilitação para Emissão de CFO e CFOC</h1>
	<br/>
	
	<fieldset class="divResponsavel block box">
		<legend>Responsável Técnico</legend>		
		<!-- Linha 1 -->
		<div class="block">
			<div class="coluna70">
				<label for="HabilitarEmissao.Responsavel.Pessoa.NomeRazaoSocial">Nome</label>
				<%= Html.TextBox("HabilitarEmissao.Responsavel.Pessoa.NomeRazaoSocial", Model.HabilitarEmissao.Responsavel.Pessoa.NomeRazaoSocial, new { @class = "txtResponsavelNome text disabled", @disabled = "disabled" })%>
			</div>
		</div>
        <!-- Linha 2 -->
		<div class="block">
            <div class="coluna20">
				<label for="HabilitarEmissao_TelefoneResidencial">Telefone residencial</label>
				<%= Html.TextBox("HabilitarEmissao.TelefoneResidencial", Model.HabilitarEmissao.TelefoneResidencial, new { @maxlength = "40", @class = "text maskFone disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna20 prepend2">
				<label for="HabilitarEmissao_TelefoneResidencial">Telefone celular</label>
				<%= Html.TextBox("HabilitarEmissao.TelefoneResidencial", Model.HabilitarEmissao.TelefoneResidencial, new { @class = "text maskFone disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna20  prepend2">
				<label for="HabilitarEmissao_TelefoneFax">Telefone fax</label>
				<%= Html.TextBox("HabilitarEmissao.TelefoneFax", Model.HabilitarEmissao.TelefoneFax, new { @maxlength = "40", @class = "text maskFone disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna20 prepend2">
				<label for="HabilitarEmissao_TelefoneComercial">Telefone comercial</label>
				<%= Html.TextBox("HabilitarEmissao.TelefoneComercial", Model.HabilitarEmissao.TelefoneComercial, new { @maxlength = "40", @class = "text maskFone disabled", @disabled = "disabled" })%>
			</div>
		</div>

        <!-- Linha 3 -->
		<div class="block">
            <div class="coluna30">
				<label for="HabilitarEmissao_Email">Email</label>
				<%= Html.TextBox("HabilitarEmissao.Email", Model.HabilitarEmissao.Email, new { @maxlength = "40", @class = "text maskFone disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna30 prepend2">
				<label for="HabilitarEmissao_RegistroCrea">Registro no CREA</label>
				<%= Html.TextBox("HabilitarEmissao.RegistroCrea", Model.HabilitarEmissao.RegistroCrea, new { @class = "text maskFone disabled", @disabled = "disabled" })%>
			</div>			
		</div>

	</fieldset>

	<fieldset class="divregistro block box">
		<legend>Praga</legend>		
		<!-- Linha 3 -->
		<div class="block">
        <div class="gridContainer">
            <table class="dataGridTable gridPraga" width="100%" border="0" cellspacing="0" cellpadding="0">
                <thead>
                    <tr>
                       <th width="20%">Nome científico</th>
							  <th width="20%">Nome comum</th>
							  <th width="20%">Cultura</th>
							  <th width="15%">Data inicial</th>
							  <th width="15%">Data final</th>
                    </tr>
                </thead>
                <tbody>
                    <%
							  Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado.PragaHabilitarEmissao item = null;
								for (int i = 0; i < Model.HabilitarEmissao.Pragas.Count; i++)
                        {
									item = Model.HabilitarEmissao.Pragas[i];                   
                    %>
                    <tr>
                        <td>
                            <label class="lblNomeCientifico" title="<%=item.Praga.NomeCientifico%>"><%=item.Praga.NomeCientifico%> </label>
                        </td>
							   <td>
                            <label class="lblNomeComun" title="<%=item.Praga.NomeComum%>"><%=item.Praga.NomeComum%> </label>
                        </td>
							   <td>
                            <label class="lblCultura" title="<%=item.Cultura%>"><%=item.Cultura%> </label>
                        </td>
							   <td>
                            <label class="lblDataInicialHabilitacao" title="<%=item.DataInicialHabilitacao%>"><%=item.DataInicialHabilitacao%> </label>
                        </td>
							   <td>
                            <label class="lblDataFinalHabilitacao" title="<%=item.DataFinalHabilitacao%>"><%=item.DataFinalHabilitacao%> </label>
                        </td>                        
                    </tr>
                    <% } %>

						  <!-- Template -->
                    <tr class="hide tr_template">
                        <td>
                            <label class="lblNomeCientifico"></label>
                        </td>
							   <td>
                            <label class="lblNomeComun"></label>
                        </td>
							   <td>
                            <label class="lblCultura"></label>
                        </td>
							   <td>
                            <label class="lblDataInicialHabilitacao"></label>
                        </td>
							   <td>
                            <label class="lblDataFinalHabilitacao"></label>
                        </td>                       
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
	</fieldset>


</div>


