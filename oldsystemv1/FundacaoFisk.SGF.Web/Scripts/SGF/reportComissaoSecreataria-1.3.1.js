var TODOS = 0;

function montarComissaoSecretaria() {
    require([
"dojo/ready",
"dojo/_base/xhr",
"dojo/store/Memory",
"dojo/on",
"dijit/form/Button",
"dojo/window"
    ], function (ready, xhr, Memory, on, Button, window) {
        ready(function () {
            sugerirDatas();
            new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(window); } }, "relatorioComissao");
            buscarComponentesFiltrosRelatorios();
            showCarregando();
            dijit.byId("cbFuncionario").on("change", function (event) {
                try {
                    if (!hasValue(event) || event < TODOS)
                        dijit.byId("cbFuncionario").set("value", TODOS);
                }
                catch (e) {
                    postGerarLog(e);
                }
            });

            dijit.byId("cbProduto").on("change", function (event) {
                try {
                    if (!hasValue(event) || event < TODOS)
                        dijit.byId("cbProduto").set("value", TODOS);
                }
                catch (e) {
                    postGerarLog(e);
                }
            });
        });
    });
}

function sugerirDatas() {
    dojo.xhr.post({
        url: Endereco() + "/util/PostDataInicialEFinalMes",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var dataCorrente = jQuery.parseJSON(data).retorno;
            if (hasValue(dataCorrente) && hasValue(dataCorrente.dt_ini_mes)) {
                //Data Inicial: Um mês antes à data do dia
                dijit.byId('dtInicial').attr("value", dataCorrente.dt_ini_mes);
                dijit.byId('dtInicial').oldValue = dijit.byId('dtInicial').value;
                //Data Final: Data do dia
                dijit.byId('dtFinal').attr("value", dataCorrente.dt_fim_mes);
                dijit.byId('dtFinal').oldValue = dijit.byId('dtFinal').value;
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function buscarComponentesFiltrosRelatorios() {
    try {
        dojo.xhr.get({
            url: Endereco() + "/api/coordenacao/getComponentesRelatorioComissaoSecretarias",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            if (data != null && hasValue(jQuery.parseJSON(data).retorno)) {
                data = jQuery.parseJSON(data).retorno;
                if (hasValue(data.Funcionarios) && data.Funcionarios.length > 0)
                    criarOuCarregarCompFiltering("cbFuncionario", data.Funcionarios, "", TODOS, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_pessoa_funcionario', 'no_fantasia', MASCULINO);
                if (hasValue(data.Produtos) && data.Produtos.length > 0)
                    criarOuCarregarCompFiltering("cbProduto", data.Produtos, "", TODOS, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_produto', 'no_produto', MASCULINO);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}


function emitirRelatorio(windowUtils) {
    try {
        var validado = true;
        apresentaMensagem('apresentadorMensagem', null);
        if (!dijit.byId("formComissaoSecretarias").validate())
            validado = false;
        if (!validado)
            return false;
        dojo.xhr.get({
            url: Endereco() + "/api/coordenacao/GeturlrelatorioComissaoSecretarias?cdFunc=" + dijit.byId("cbFuncionario").value + "&cd_produto=" + dijit.byId("cbProduto").value +
                "&dtInicial=" + dojo.byId('dtInicial').value + "&dtFinal=" + dojo.byId('dtFinal').value,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioComissaoSecretarias?' + data, '1024px', '750px', 'popRelatorio');
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}
