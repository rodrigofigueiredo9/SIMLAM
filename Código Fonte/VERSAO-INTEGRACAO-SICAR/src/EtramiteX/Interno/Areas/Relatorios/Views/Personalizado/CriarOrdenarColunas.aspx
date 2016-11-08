<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Criar Relatório Personalizado - Ordenar Colunas</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">


<script language="javascript" type="text/javascript">
    <!--
        $(function () {

            ///Deixa a largura dos itens da Wizard distribuida de forma igual
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

            /////Botões
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
        });
    //-->
</script>



</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="central">

        <h1 class="titTela">Criar Relatórios Personalizados</h1>

        <div class="block">
            <div class="wizBar coluna100">
                <ul>
                    <li class="anterior"><span>Opções</span></li>
                    <li class="ativo"><span>Ordenar Colunas</span></li>
                    <li class=""><span>Ordenar Valores</span></li>
                    <li class=""><span>Filtros</span></li>
                    <li class=""><span>Sumarização</span></li>
                    <li class=""><span>Dimensionar</span></li>
                    <li class=""><span>Agrupar</span></li>
                    <li class=""><span>Salvar</span></li>
                </ul>
            </div><!-- .wizBar -->
        </div>

        <div class="block box">
            <div class="coluna25 floatRight alinhaDireita">
                <button class="btAvancar">Avançar</button>
            </div>
            <div class="coluna25">
                <button class="btVoltar">Voltar</button>
            </div>
        </div>

        <div class="mensagemSistema ajuda">
            <div class="textoMensagem textoAjuda">
                <a class="fecharMensagem" title="Fechar Mensagem">Fechar Mensagem</a>
                <h5>Ordenação em Grupo</h5>

                <p>Clique nos itens segurando a tecla SHIFT do seu teclado para agrupar os itens e arraste para ordenar o grupo.</p>

            </div>
        </div><!-- .ajuda -->

        <h5>Ordenar Colunas do Relatório</h5>

        <div class="block">
            <div class="coluna50">
                <ul class="sortable">
                    <li><span>Título – Número</span></li>
                    <li><span>Título – Data de vencimento</span></li>
                    <li><span>Processo – Número</span></li>
                    <li><span>Interessado – Nome</span></li>
                    <li><span>Empreendimento  - Razão social</span></li>
                </ul>
            </div>
        </div>

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
