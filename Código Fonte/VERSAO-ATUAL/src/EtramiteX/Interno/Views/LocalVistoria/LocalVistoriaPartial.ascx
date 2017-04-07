<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMLocalVistoria" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<LocalVistoriaVM>" %>

<fieldset class="block box">
	<legend></legend>
	    <div class="block">
            <div class="coluna62">
				    <label for="SetorTipo">Setor</label>
				    <%= Html.DropDownList("SetorTipo", Model.Setores, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlSetores" }))%>
		    </div>
        </div>

    <% if (!Model.IsVisualizar) { %>

        <div class="block hide" style="display: block;">
            <div class="coluna20">
				    <label for="EditandoDiaSemanaTexto">Dia da Semana</label>
                        <%= Html.DropDownList("DiaSemana", Model.ListaDiasSemana, new { @class = "text ddldiasemana" })%>

		    </div>
            <div class="coluna20">
				    <label for="EditandoDiaSemanaTexto">Hora de Início</label>
                        <%= Html.TextBox("HoraInicio","", new { @class = "txtHoraInicio text maskHoraMinuto" })%>
		    </div>
            <div class="coluna20">
				    <label for="EditandoDiaSemanaTexto">Hora de Fim</label>
                        <%= Html.TextBox("HoraFim", "" , new { @class = "txtHoraFim text maskHoraMinuto" })%>
                        <%= Html.Hidden("hdnId", "", new { @class = "hdnId" })%>
                        <%= Html.Hidden("hdnLocalVistoriaId","", new { @class = "hdnLocalVistoriaId" })%>
                        <%= Html.Hidden("hdnLocalVistoriaTid", "", new { @class = "hdnLocalVistoriaTid" })%>
                        <%= Html.Hidden("hdnLocalVistoriaSituacao","", new { @class = "hdnLocalVistoriaSituacao" })%>
		    </div>

		    <div class="coluna10">
			    <button class="inlineBotao btnAdicionar">Adicionar</button>
			    <button class="inlineBotao btnEditar hide">Editar</button>
		    </div>

        </div>
		
    <%} %>
    	<div class="block">
		<div class="gridContainer">
			<table class="dataGridTable gridLocalVistoria" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th width="20%">Dia da Semana</th>
                        <th width="20%">Hora Inicio</th>
                        <th width="20%">Hora Fim</th>
                       
                        <% if (!Model.IsVisualizar) { %>
						    <th class="semOrdenacao" width="15%">Ações</th>
                        <% } %>
					</tr>
				</thead>
				<tbody>
					<% Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria.DiaHoraVistoria item = null;
						for (int i = 0; i < Model.DiasHorasVistoria.Count; i++)  {
							item = Model.DiasHorasVistoria[i]; 
					%>
						<tr>
							<td>
                                <label class="lblDiaSemana" id="lblDiaSemana"><%= item.DiaSemanaTexto %> </label>
                                <input type="hidden" class="hdnDiaSemanaId" value="<%= item.DiaSemanaId %>" />
							</td>
							<td>
                                <label class="lblHoraInicio" id="lblHoraInicio"><%= item.HoraInicio %></label>
							</td>
							<td>
                                <label class="lblHoraFim" id="lblHoraFim"><%= item.HoraFim %></label>
							</td>
                            <% if (!Model.IsVisualizar) { %>
							    <td>
                                    <a class="icone editar btnItemEditar" title="Editar"></a>
                                    <a class="icone excluir btnExcluir" title="Excluir"></a>
							        <input type="hidden" value="<%= item.Id %>" class="hdnItemId" />
                                    <input type="hidden" value="<%= item.Tid %>" class="hdnItemTid" />
							        <input type="hidden" value="<%= i %>" class="hdnItemIndex" />
							    </td>
                            <% } %>
						</tr>
					<% } %>
                    <% if (!Model.IsVisualizar) { %>
					<tr class="hide tr_template">
						<td>
							<label class="lblDiaSemana" id="lblDiaSemana"></label>
                            <input type="hidden" value="" class="hdnDiaSemanaId" />
						</td>
						<td>
							<label class="lblHoraInicio" id="lblHoraInicio"></label>
						</td>
						<td>
							<label class="lblHoraFim" id="lblHoraFim"></label>
						</td>
						<td>
                            <a class="icone editar btnItemEditar" title="Editar"></a>
                            <a class="icone excluir btnExcluir" title="Excluir"></a>
							<input type="hidden" value="" class="hdnItemId" />
                            <input type="hidden" value="" class="hdnItemTid" />
							<input type="hidden" value="" class="hdnItemIndex" />
						</td>
					</tr>
                    <% } %>
				</tbody>
			</table>
		</div>
	</div>
    </fieldset>
    <fieldset class="block box">
        <legend>Bloqueios</legend>
      <% if (!Model.IsVisualizar) { %>
        
        <div class="block hide" style="display: block;">
            <div class="coluna20">
				    <label for="DataInicialBloqueio">Data Inicial</label>
                        <%= Html.TextBox("DataInicialBloqueio","", new { @class = "txtDataInicialBloqueio datepicker maskData text" })%>
		    </div>

             <div class="coluna20">
				    <label for="HoraInicialBloqueio">Hora Inicial</label>
                        <%= Html.TextBox("HoraInicialBloqueio","", new { @class = "txtHoraInicialBloqueio text maskHoraMinuto" })%>
		    </div>
            <div class="coluna20">
				    <label for="DataFinalBloqueio">Data Final</label>
                        <%= Html.TextBox("DataFinalBloqueio","", new { @class = "txtDataFinalBloqueio datepicker maskData text" })%>
		    </div>

             <div class="coluna20">
				    <label for="HoraFinallBloqueio">Hora Final</label>
                        <%= Html.TextBox("HoraFinalBloqueio","", new { @class = "txtHoraFinalBloqueio text maskHoraMinuto" })%>
		    </div>
                      
		    <div class="coluna10">
			    <button class="inlineBotao btnAdicionarBloqueio">Adicionar</button>

		    </div>

        </div>
		
    <%} %>
        
    	<div class="block">
		<div class="gridContainer">
			<table class="dataGridTable gridBloqueios" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th width="20%">Data Inicial</th>
                        <th width="20%">Hora Inicial</th>
                        <th width="20%">Data Final</th>
                        <th width="20%">Hora Final</th>
                        <% if (!Model.IsVisualizar) { %>
						    <th class="semOrdenacao" width="15%">Ações</th>
                        <% } %>
					</tr>
				</thead>
				<tbody>
					<% Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria.BloqueioLocalVistoria itemBloqueio = null;
						for (int i = 0; i < Model.ListBloqueios.Count; i++)  {
                            itemBloqueio = Model.ListBloqueios[i];
                            var DataInicial = itemBloqueio.DiaInicio.ToString("dd/MM/yyyy");
                            var HoraInicial = itemBloqueio.HoraInicio;
                            var DataFinal = itemBloqueio.DiaFim.ToString("dd/MM/yyyy");
                            var HoraFinal = itemBloqueio.HoraFim;
					%>
						<tr>
							<td>
                                <label class="lblDataInicialBloqueio" id="Label1"><%= DataInicial %> </label>
                              
							</td>
							<td>
                                <label class="lblHoraInicialBloqueio" id="Label2"><%= HoraInicial%></label>
							</td>
							<td>
                                <label class="lblDataFinalBloqueio" id="Label3"><%= DataFinal %> </label>
                              
							</td>
							<td>
                                <label class="lblHoraFinalBloqueio" id="Label7"><%= HoraFinal %></label>
							</td>
                            <% if (!Model.IsVisualizar) { %>
							    <td>
                                    <a class="icone excluir btnExcluir" title="Excluir"></a>
							        <input type="hidden" value="<%= item.Id %>" class="hdnItemBloqueioId" />
                                    <input type="hidden" value="<%= item.Tid %>" class="hdnItemBloqueioTid" />
							        <input type="hidden" value="<%= i %>" class="hdnItemBloqueioIndex" />
							    </td>
                            <% } %>
						</tr>
					<% } %>
                    <% if (!Model.IsVisualizar) { %>
					<tr class="hide tr_template_bloqueio">
						<td>
							<label class="lblDataInicialBloqueio" id="lblDataInicialBloqueio"></label>                          
						</td>
						<td>
							<label class="lblHoraInicialBloqueio" id="lblHoraInicialBloqueio"></label>
						</td>
						<td>
							<label class="lblDataFinalBloqueio" id="lblDataFinalBloqueio"></label>
						</td>
						<td>
							<label class="lblHoraFinalBloqueio" id="lblHoraFinalBloqueio"></label>

						</td>
						<td>
                            <a class="icone excluir btnExcluirBloqueio" title="Excluir"></a>
							<input type="hidden" value="" class="hdnItemBloqueioId" />
                            <input type="hidden" value="" class="hdnItemBloqueioTid" />
							<input type="hidden" value="" class="hdnItemBloqueioIndex" />
						</td>
					</tr>
                    <% } %>
				</tbody>
			</table>
		</div>
	</div>


</fieldset>