<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFichaFundiaria" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FichaFundiariaVM>" %>

<input type="hidden" class="hdnCodigo" value="<%:Model.FichaFundiaria.Codigo%>" />

<fieldset class="box">
	<legend>Protocolo</legend>

	<div class="block">
		<div class="coluna40 append2">
			<label for="FichaFundiaria_ProtocoloGeral">Protocolo geral</label>
			<%= Html.TextBox("FichaFundiaria.ProtocoloGeral", Model.FichaFundiaria.ProtocoloGeral, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtProtocoloGeral", @maxlength="300" }))%>
		</div>

		<div class="coluna40">
			<label for="FichaFundiaria_ProtocoloRegional">Protocolo regional</label>
			<%= Html.TextBox("FichaFundiaria.ProtocoloRegional", Model.FichaFundiaria.ProtocoloRegional, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtProtocoloRegional", @maxlength = "300" }))%>
		</div>
	</div>
</fieldset>

<fieldset class="box">
	<legend>Requerente</legend>

	<div class="block">
		<div class="coluna83">
			<label for="FichaFundiaria_Requerente_Nome">Nome</label>
			<%= Html.TextBox("FichaFundiaria.Requerente.Nome", Model.FichaFundiaria.Requerente.Nome, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtRequerenteNome", @maxlength = "300" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna30 append2">
			<label for="FichaFundiaria_Requerente_DocumentoTipo">Tipo do documento</label>
			<%= Html.TextBox("FichaFundiaria.Requerente.DocumentoTipo", Model.FichaFundiaria.Requerente.DocumentoTipo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtRequerenteDocumentoTipo", @maxlength = "300" }))%>
		</div>

		<div class="coluna30">
			<label for="FichaFundiaria_Requerente_DocumentoNumero">Número do documento</label>
			<%= Html.TextBox("FichaFundiaria.Requerente.DocumentoNumero", Model.FichaFundiaria.Requerente.DocumentoNumero, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtRequerenteDocumentoNumero", @maxlength = "300" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna83">
			<label for="FichaFundiaria_Requerente_NomePai">Pai</label>
			<%= Html.TextBox("FichaFundiaria.Requerente.NomePai", Model.FichaFundiaria.Requerente.NomePai, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtRequerenteNomePai", @maxlength = "300" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna83">
			<label for="FichaFundiaria_Requerente_NomeMae">Mãe</label>
			<%= Html.TextBox("FichaFundiaria.Requerente.NomeMae", Model.FichaFundiaria.Requerente.NomeMae, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtRequerenteNomeMae", @maxlength = "300" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna83">
			<label for="FichaFundiaria_Requerente_Endereco">Endereço</label>
			<%= Html.TextArea("FichaFundiaria.Requerente.Endereco", Model.FichaFundiaria.Requerente.Endereco, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtRequerenteEndereco", @maxlength = "300" }))%>
		</div>
	</div>
</fieldset>

<fieldset class="box">
	<legend>Terreno</legend>

	<div class="block">
		<div class="coluna83">
			<label for="FichaFundiaria_Terreno_Municipio">Município</label>
			<%= Html.TextBox("FichaFundiaria.Terreno.Municipio", Model.FichaFundiaria.Terreno.Municipio, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTerrenoMunicipio", @maxlength = "300" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna83">
			<label for="FichaFundiaria_Terreno_Distrito">Distrito</label>
			<%= Html.TextBox("FichaFundiaria.Terreno.Distrito", Model.FichaFundiaria.Terreno.Distrito, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTerrenoDistrito", @maxlength = "300" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna83">
			<label for="FichaFundiaria_Terreno_Lugar">Lugar</label>
			<%= Html.TextBox("FichaFundiaria.Terreno.Lugar", Model.FichaFundiaria.Terreno.Lugar, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTerrenoLugar", @maxlength = "300" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna26 append2">
			<label for="FichaFundiaria_Terreno_Tipo">Tipo de terreno</label>
			<%= Html.TextBox("FichaFundiaria.Terreno.Tipo", Model.FichaFundiaria.Terreno.Tipo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTerrenoTipo", @maxlength = "300" }))%>
		</div>

		<div class="coluna26 append2">
			<label for="FichaFundiaria_Terreno_DataMedicao">Data da medição</label>
			<%= Html.TextBox("FichaFundiaria.Terreno.DataMedicao", Model.FichaFundiaria.Terreno.DataMedicao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTerrenoDataMedicao", @maxlength = "300" }))%>
		</div>

		<div class="coluna25">
			<label for="FichaFundiaria_Terreno_Area">Área medida (m²)</label>
			<%= Html.TextBox("FichaFundiaria.Terreno.Area", Model.FichaFundiaria.Terreno.Area, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTerrenoArea", @maxlength = "300" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna26 append2">
			<label for="FichaFundiaria_Terreno_Perimetro">Perímetro (ml)</label>
			<%= Html.TextBox("FichaFundiaria.Terreno.Perimetro", Model.FichaFundiaria.Terreno.Perimetro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTerrenoPerimetro", @maxlength = "300" }))%>
		</div>

		<div class="coluna26 append2">
			<label for="FichaFundiaria_Terreno_Lote">Lote</label>
			<%= Html.TextBox("FichaFundiaria.Terreno.Lote", Model.FichaFundiaria.Terreno.Lote, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTerrenoLote", @maxlength = "300" }))%>
		</div>

		<div class="coluna25">
			<label for="FichaFundiaria_Terreno_Quadra">Quadra</label>
			<%= Html.TextBox("FichaFundiaria.Terreno.Quadra", Model.FichaFundiaria.Terreno.Quadra, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTerrenoQuadra", @maxlength = "300" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna83">
			<label for="FichaFundiaria_Terreno_NomeTopografo">Topógrafo</label>
			<%= Html.TextBox("FichaFundiaria.Terreno.NomeTopografo", Model.FichaFundiaria.Terreno.NomeTopografo, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTerrenoNomeTopografo", @maxlength = "300" }))%>
		</div>
	</div>
</fieldset>

<fieldset class="box">
	<legend>Confrontantes</legend>

	<div class="block">
		<div class="coluna83">
			<label for="FichaFundiaria_ConfrontanteNorte">Norte</label>
			<%= Html.TextArea("FichaFundiaria.ConfrontanteNorte", Model.FichaFundiaria.ConfrontanteNorte, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontanteNorte", @maxlength = "300" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna83">
			<label for="FichaFundiaria_ConfrontanteSul">Sul</label>
			<%= Html.TextArea("FichaFundiaria.ConfrontanteSul", Model.FichaFundiaria.ConfrontanteSul, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontanteSul", @maxlength = "300" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna83">
			<label for="FichaFundiaria_ConfrontanteLeste">Leste</label>
			<%= Html.TextArea("FichaFundiaria.ConfrontanteLeste", Model.FichaFundiaria.ConfrontanteLeste, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontanteLeste", @maxlength = "300" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna83">
			<label for="FichaFundiaria_ConfrontanteOeste">Oeste</label>
			<%= Html.TextArea("FichaFundiaria.ConfrontanteOeste", Model.FichaFundiaria.ConfrontanteOeste, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtConfrontanteOeste", @maxlength = "300" }))%>
		</div>
	</div>
</fieldset>

<fieldset class="box">
	<legend>Escritura</legend>

	<fieldset class="boxBranca coluna37">
		<legend>Escritura condicional</legend>

		<div class="block">
			<div class="coluna93">
				<label for="FichaFundiaria_EscrituraCondicional_Data">Data</label>
				<%= Html.TextBox("FichaFundiaria.EscrituraCondicional.Data", Model.FichaFundiaria.EscrituraCondicional.Data, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEscrituraCondicionalData", @maxlength = "300" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna93">
				<label for="FichaFundiaria_EscrituraCondicional_Livro">Livro</label>
				<%= Html.TextBox("FichaFundiaria.EscrituraCondicional.Livro", Model.FichaFundiaria.EscrituraCondicional.Livro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEscrituraCondicionalLivro", @maxlength = "300" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna93">
				<label for="FichaFundiaria_EscrituraCondicional_Folha">Folha</label>
				<%= Html.TextBox("FichaFundiaria.EscrituraCondicional.Folha", Model.FichaFundiaria.EscrituraCondicional.Folha, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEscrituraCondicionalFolha", @maxlength = "300" }))%>
			</div>
		</div>
	</fieldset>

	<fieldset class="boxBranca coluna37">
		<legend>Escritura definitiva</legend>

		<div class="block">
			<div class="coluna93">
				<label for="FichaFundiaria_EscrituraDefinitiva_Data">Data</label>
				<%= Html.TextBox("FichaFundiaria.EscrituraDefinitiva.Data", Model.FichaFundiaria.EscrituraDefinitiva.Data, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEscrituraDefinitivaData", @maxlength = "300" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna93">
				<label for="FichaFundiaria_EscrituraDefinitiva_Livro">Livro</label>
				<%= Html.TextBox("FichaFundiaria.EscrituraDefinitiva.Livro", Model.FichaFundiaria.EscrituraDefinitiva.Livro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEscrituraDefinitivaLivro", @maxlength = "300" }))%>
			</div>
		</div>

		<div class="block">
			<div class="coluna93">
				<label for="FichaFundiaria_EscrituraDefinitiva_Folha">Folha</label>
				<%= Html.TextBox("FichaFundiaria.EscrituraDefinitiva.Folha", Model.FichaFundiaria.EscrituraDefinitiva.Folha, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEscrituraDefinitivaFolha", @maxlength = "300" }))%>
			</div>
		</div>
	</fieldset>
</fieldset>

<fieldset class="box">
	<legend>Observações</legend>

	<div class="block">
		<div class="coluna83">
			<%= Html.TextArea("FichaFundiaria.Observacoes", Model.FichaFundiaria.Observacoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text media txtObservacoes", @maxlength = "300" }))%>
		</div>
	</div>
</fieldset>
