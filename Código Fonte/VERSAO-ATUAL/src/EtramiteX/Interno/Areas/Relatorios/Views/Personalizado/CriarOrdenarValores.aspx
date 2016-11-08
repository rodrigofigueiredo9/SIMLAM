<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Criar Relatório Personalizado - Ordenar Valores</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
<script language="javascript" type="text/javascript">
    <!--
    $(function () {
        //Deixa a largura dos itens da Wizard distribuida de forma igual
        $('.wizBar ul li').css('width', 100 / $('.wizBar li').length + '%');

        ///Inicia a Lista Ordenável
        $('.sortable').sortable({
            placeholder: 'holder',
            start: function (event, ui) {

                //PlaceHolder prejudica a altura do elemento, é necessário configurar manualmente
                $('.holder').css('height', ui.item.height());

                if (ui.item.hasClass('grouped')) {
                    ui.item.data('i', $('.sortable .grouped').index(ui.item));
                    $('.sortable .grouped:not(.holder)').not(ui.item).each(function () {
                        $(this).data('n', $('.sortable li:not(.holder)').index(this));
                    }).appendTo('#helper');

                    //Não deixa que os itens '.grouped' continue indexado
                    $('.sortable').sortable('refresh');

                    $('#helper').show();

                    //Ajusta o PlaceHolder para ter a altura dos objetos
                    $('.holder').css('height', (($('#helper li').length + 1) * ui.item.outerHeight()) + 'px');

                    //Cancelar a ordenação em grupo
                    $(window).one('keyup', function (e) {
                        if (e.keyCode == 27) { // esc. PANIC!
                            $('#helper li').each(function () {
                                if ($('.sortable li:not(.holder)').length > $(this).data('n')) { //Pode ser o último item
                                    $(this).insertBefore($('.sortable li:not(.holder)')[$(this).data('n')]);
                                } else { //Se é o último ele é posicionado de volta.
                                    $('.sortable').append(this)
                                }
                            });
                            $('.sortable').sortable('refresh');
                            //Altura do PlaceHolder precisa ser reconfigurada
                            $('.holder').css('height', ui.item.height());
                        } // if
                    });
                }
            },
            //Tenta atualizar mesmo se o DOM não mudar
            stop: function (event, ui) {
                if ($('#helper li').length) {
                    $('#helper').hide();
                    ui.item.after($('#helper li'));

                    var pos = ui.item.data('i');
                    if (pos > 0) ui.item.insertAfter($('.sortable .grouped')[pos]);

                    //Confirma se o 'sortable' sabe sobre os itens '.grouped' realocados
                    $('.sortable').sortable('refresh');
                }
                $('.sortable li').removeClass('grouped');
            },
            sort: function (event, ui) { //Move o 'helper' com o item sendo ordenado
                if ($('#helper li').length) {
                    var offset = ui.item.offset();
                    $('#helper').css({
                        left: (offset.left - 43) + 'px',
                        top: (offset.top + 10) + 'px'
                    });
                }
            },
            remove: function (event, ui) { //Adiciona mensagem quando a lista fica vazia
                if ($(this).find('li').length == 0) {
                    $(this).html('<span class="quiet">Arraste os item aqui.</span>');
                    $(this).addClass('box dashed');
                }
            },
            receive: function (event, ui) { //Remove mensagem quando a lista não é mais vazia
                $(this).find('span.quiet').remove();
                $(this).removeClass('box dashed');
            },
            connectWith: ".sortable",
            revert: 100,
            opacity: 0.6,
            containment: 'document'
        }).disableSelection();

        //Agrupa os itens quando com a tecla SHIFT .
        $('.sortable li').live('click', function (e) {
            if (e.shiftKey) {
                $(this).toggleClass('grouped');
            }
        });

        $('body').append($('<ul id="helper">'));

        //Botão de Ordem alfabética
        $('.alfaOrder').mouseup(function () {
            $(this).toggleClass('ativo');

            var legAlfa;
            if ($(this).is('.ativo')) {
                legAlfa = 'Z > A';
            } else {
                legAlfa = 'A > Z';
            }
            $(this).text(legAlfa);
        });

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Botões Jquery
        $('.btAvancar').button({
            icons: {
                secondary: 'ui-icon-avancar'
            }
        });
        $('.btVoltar').button({
            icons: {
                primary: 'ui-icon-voltar'
            }
        });
        $('.btVisualizar').button({
            icons: {
                primary: 'ui-icon-visualizar'
            }
        });


        var top = $('.alvoValores').offset().top - parseFloat($('.alvoValores').css('marginTop').replace(/auto/, 0)) - 55;
        $(window).scroll(function () {
            fixaAlvo();
        });
        $(window).resize(function () {
            fixaAlvo();
        });
        function fixaAlvo() {

            if ($('.alvoValores').height() < ($(window).height() - 160)) {
                var y = $(this).scrollTop();
                if (y >= top) {
                    $('.alvoValores').addClass('fixed');
                } else {
                    $('.alvoValores').removeClass('fixed');
                }
            } else {
                $('.alvoValores').removeClass('fixed');
            }
        }
    });
    //-->
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="central">

    <h1 class="titTela">Criar Relatórios Personalizados</h1>

    <!-- ========================================================================= -->
    <!-- ========================================================================= -->

    <div class="block">
        <div class="wizBar coluna100">
            <ul>
                <li class="anterior"><span>Opções</span></li>
                <li class="anterior"><span>Ordenar Colunas</span></li>
                <li class="ativo"><span>Ordenar Valores</span></li>
                <li class=""><span>Filtros</span></li>
                <li class=""><span>Sumarização</span></li>
                <li class=""><span>Dimensionar</span></li>
                <li class=""><span>Agrupar</span></li>
                <li class=""><span>Salvar</span></li>
            </ul>
        </div><!-- .wizBar -->
    </div>

    <!-- ========================================================================= -->

    <div class="block box">
        <div class="coluna25 floatRight alinhaDireita">
            <button class="btAvancar">Avançar</button>
        </div>
        <div class="coluna25">
            <button class="btVoltar">Voltar</button>
        </div>
    </div>

    <!-- ========================================================================= -->



    <!-- ========================================================================= -->
    <!-- ========================================================================= -->

    <!-- MENSAGEM DE TUTORIAL/AJUDA ========================================================== -->
    <div class="mensagemSistema ajuda">
        <div class="textoMensagem textoAjuda">
            <a class="fecharMensagem" title="Fechar Mensagem">Fechar Mensagem</a>
            <h5>Ordenação em Grupo</h5>
            <ul>
                <li>Clique nos itens segurando a tecla SHIFT do seu teclado para agrupar os itens e arraste para ordenar o grupo.</li>
                <li>Clique no botão A>Z para inverter a ordem alfabética.</li>
            </ul>
        </div>
    </div><!-- .ajuda -->
    <!-- ========================================================================= -->

    <div class="block margemDTop borderB padding0">
        <div class="coluna25">
            <p class="">
                <label for="dummy3">Agrupar por Dimensão</label><br>
                <select name="dummy3">
                    <option>Título</option>
                    <option>Modelo</option>
                    <option>Processo</option>
                    <option>Documento</option>
                    <option>Empreendimentos</option>
                    <option>Interessados</option>
                    <option>Data de Início do prazo</option>
                </select>
            </p>
        </div>
    </div>
    <!-- ========================================================================= -->
    <!-- ========================================================================= -->
    <h5>Odenar Valores das Colunas</h5>
    <div class="block margem0">
        <div class="coluna50 ">
            <strong>Coluna A</strong>
            <ul class="sortable">
                <li><span>Titulo - Autor</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Titulo - Número</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Titulo - Data de conclusão</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Titulo - Motivo do encerramento</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Titulo - Setor</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Titulo - Data de emissão</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Titulo - Dias prorrogados</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Titulo - Prazo</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Titulo - Data de Criação</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Titulo - Data de Assinatura</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Titulo - Data de Encerramento</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Titulo - Data de Vencimento</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Titulo - Situação</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Titulo - Local da Emissão</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>

                <li><span>Modelo - Nome</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Modelo - Subtipo</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Modelo - Sigla</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Modelo - Situação</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>

                <li><span>Processo - Número</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Processo - Tipo</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Processo - Data de Criação</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>

                <li><span>Documento - Número</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Documento - Tipo</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Documento - Data de Criação</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Documento - Nome</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>

                <li><span>Empreendimento - Razão Social</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Empreendimento - Atividade Principal</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Empreendimento - Segmento</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Empreendimento - Área Total</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Empreendimento - Nome Fansasia</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Empreendimento - CNPJ</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>

                <li><span>Interessados - Nome</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Interessados - Razão Social</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Interessados - CPF</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Interessados - CNPJ</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>

                <li><span>Data de Início do prazo - Data</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Data de Início do prazo - Dia</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Data de Início do prazo - Mês</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>
                <li><span>Data de Início do prazo - Ano</span><span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span></li>

            </ul>
        </div>
        <div class="coluna48 ultima posRelative">
            <strong>Coluna B</strong>
            <ul class="sortable box dashed alvoValores">
                <span class="quiet">Arraste os item aqui.</span>
            </ul>
        </div>
    </div>

    <!-- ========================================================================= -->
    <!-- ========================================================================= -->

    <div class="block box margemDTop">
        <div class="coluna25 floatRight alinhaDireita">
            <span class="cancelarCaixaDir">ou <a class="linkCancelar" title="Cancelar">Cancelar</a></span>
            <button class="btAvancar">Avançar</button>
        </div>
        <div class="coluna25">
            <button class="btVoltar">Voltar</button>
        </div>
    </div>


</div>

</asp:Content>
