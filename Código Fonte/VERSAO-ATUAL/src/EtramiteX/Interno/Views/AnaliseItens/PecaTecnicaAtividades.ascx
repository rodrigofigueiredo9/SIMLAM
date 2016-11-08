<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.AnaliseItens.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PecaTecnicaVM>" %>

<fieldset class="block box" id="fsAtividadeSolicitada">
	<legend>Atividade Solicitada</legend>
	
	<table class="dataGridTable tabAtividadeSolicitada">	
		<tbody>
			<% foreach (var atividade in Model.PecaTecnica.Protocolo.Atividades)
	  { %>
				<tr>
					<td>
						<label>
							<input type="radio" class="radioAtividade" title="<%: atividade.NomeAtividade %>" name="atividadesSolicitadas" value="<%: atividade.IdRelacionamento %>"/>
							<%: atividade.NomeAtividade %>
						</label>
					</td>
				</tr>
			<% } %>
		</tbody>
	</table>
</fieldset>
<input type="hidden" class="protocoloSelecionadoId" />
<div class="divConteudo" >
	
</div>