<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities.IndicadorPeriodoRelatorio>" %>

<div class="caixaBarra">
	<span>No próximo mês</span>
	<div class="barraAuto coluna<%=	(Convert.ToInt32(((Model.ProximoMes/Model.Total)*100))>0)?Convert.ToInt32(((Model.ProximoMes/Model.Total)*99)):4 %>">
		<p class="barraLegenda"><%= Model.ProximoMes%></p>
	</div>
</div>

<div class="caixaBarra append5">
	<span>Neste mês</span>
	<div class="barraAuto coluna<%=	(Convert.ToInt32(((Model.EsseMes/Model.Total)*100))>0)?Convert.ToInt32(((Model.EsseMes/Model.Total)*99)):4 %>">
		<p class="barraLegenda"><%= Model.EsseMes%></p>
	</div>
</div>
							
<div class="caixaBarra">
	<span>Nesta semana</span>
	<div class="barraAuto coluna<%=	(Convert.ToInt32(((Model.EssaSemana/Model.Total)*100))>0)?Convert.ToInt32(((Model.EssaSemana/Model.Total)*99)):4 %>">
		<p class="barraLegenda"><%= Model.EssaSemana%></p>
	</div>
</div>

<div class="caixaBarra">
	<span>Hoje</span>
	<div class="barraAuto coluna<%=	(Convert.ToInt32(((Model.Hoje/Model.Total)*100))>0)?Convert.ToInt32(((Model.Hoje/Model.Total)*99)):4 %>">
		<p class="barraLegenda"><%= Model.Hoje%></p>
	</div>
</div>