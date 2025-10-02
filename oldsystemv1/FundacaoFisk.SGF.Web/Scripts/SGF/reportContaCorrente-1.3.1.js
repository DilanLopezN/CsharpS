function montarMetodosContaCorrente() {
    //Criação dos componentes
    require([
    "dojo/ready",
    "dojo/on",
    "dojo/date",
    "dijit/form/Button",
    "dojo/dom-attr",
    ], function (ready, on, date, Button) {
        ready(function () {
            try {
                motarDropsPesquisaLocalMovto();
                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        emitirRelatorio();
                    }
                }, "relatorio");
                adicionarAtalhoPesquisa(['localMovto', 'tipoLiquidacao', 'dtInicialCC', 'dtFinalCC'], 'relatorio', ready);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323083', '765px', '771px');
                        });
                }

            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}
//#region  motarDropsPesquisaLocalMovto
function motarDropsPesquisaLocalMovto() {
	showCarregando();
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/financeiro/getLocalMovtoWithContaByEscola",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataMovto) {
            try {
	            //showCarregando();
                var dados = dataMovto.retorno;
                dijit.byId('dtInicialCC').attr("value", new Date(ano, mes, dia));
                if (dados != null) {
                    apresentaMensagem("apresentadorMensagem", null);
                    loadMovto(dados.localMovimento, 'localMovto');
                    loadTipoLiquidacao(dados.tipoLiquidacao, 'tipoLiquidacao');
                    var parametros = getParamterosURL();
                    if (hasValue(parametros['idEmitir']))
                        configuraFiltros();
                }
                hideCarregando();
            }
            catch (e) {
	            hideCarregando();
                postGerarLog(e);
            }
        },
            function (error) {
	            hideCarregando();
                apresentaMensagem('apresentadorMensagem', error);
        });
    });
}

function loadMovto(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
      try{
          var itemsCb = [];
          var cbMovto = dijit.byId(field);
          Array.forEach(items, function (value, i) {
              itemsCb.push({
                  id: value.cd_local_movto,
                  name: value.nomeLocal == null ? value.no_local_movto : value.nomeLocal
              });
          });
          var stateStore = new Memory({
              data: itemsCb
          });
          cbMovto.store = stateStore;
      }
      catch (e) {
          postGerarLog(e);
      }
  });
}

function loadTipoLiquidacao(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
      try{
          var itemsCb = [];
          var cbTipo = dijit.byId(field);
          Array.forEach(items, function (value, i) {
              itemsCb.push({
                  id: value.cd_tipo_liquidacao,
                  name: value.dc_tipo_liquidacao
              });
          });
          var stateStore = new Memory({
              data: itemsCb
          });

          cbTipo.store = stateStore;

          cbTipo.store.add({
              id: 0,
              name: "Todas",
              idName: "Todas"
          });

          dijit.byId(field).set("value", 0);
      }
      catch (e) {
          postGerarLog(e);
      }
  });
}

//#endregion
function emitirRelatorio() {
    try{
        var dataInicial = dijit.byId('dtInicialCC').get('value');
        var dataFinal = dijit.byId('dtFinalCC').get('value');

        if (!dijit.byId("formRptContaCorrente").validate())
            validado = false;

        if (!hasValue(dataFinal) && hasValue(dataInicial))
            dijit.byId('dtFinalCC').set('value', dataInicial);

        if (!validarDatas())
            return false;

        require(["dojo/_base/xhr"], function (xhr) {
            var Conta = new ContaCorrente();
            xhr.get({
                url: Endereco() + "/api/financeiro/getUrlRelatorioContaCorrente?cd_local_movto=" + Conta.localMovto + "&dta_ini=" + Conta.dtaIni + "&dta_fim=" + Conta.dtaFim + "&tipoLiquidacao=" + Conta.tipoLiquidacao + "&desTipoLiquidacao=" + Conta.desTipoLiquidacao,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                abrePopUp(Endereco() + '/Relatorio/RelatorioContaCorrente?' + data, '1024px', '750px', 'popRelatorio');
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarDatas() {
    try{
        apresentaMensagem("apresentadorMensagem", "");
        var dataInicial = dijit.byId('dtInicialCC').get('value');
        var dataFinal = dijit.byId('dtFinalCC').get('value');
        if (dojo.date.compare(dataInicial, dataFinal, "date") == 1) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicalMaior);
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            return false;
        }
        return true;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function ContaCorrente() {
    try{
        this.localMovto = dijit.byId('localMovto').get('value');
        this.dtaIni = hasValue(dojo.byId('dtInicialCC').value) ? dojo.byId('dtInicialCC').value : '';
        this.dtaFim = hasValue(dojo.byId('dtFinalCC').value) ? dojo.byId('dtFinalCC').value : '';
        this.tipoLiquidacao = dijit.byId('tipoLiquidacao').get('value');
        this.desTipoLiquidacao = dojo.byId('tipoLiquidacao').value;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraFiltros() {
    var parametros = getParamterosURL();
    if (hasValue(parametros['localMovto']))
        dijit.byId('localMovto').set('value', parseInt(parametros['localMovto']));
    if (hasValue(parametros['tipoLiquidacao']))
        dijit.byId('tipoLiquidacao').set('value', parseInt(parametros['tipoLiquidacao']));
    if (hasValue(parametros['data'])) {
        dojo.byId('dtInicialCC').value =  parametros['data'];
        dojo.byId('dtFinalCC').value = parametros['data'];
    }

}