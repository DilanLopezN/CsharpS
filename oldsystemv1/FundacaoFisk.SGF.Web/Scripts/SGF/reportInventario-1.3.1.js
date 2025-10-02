//#region criando dropDowns, contantes

function montarTipoRelatorio(Memory, on) {
    try{
        var dados = [{ name: "Mostrar todos os itens", id: "0" },
                    { name: "Mostrar somente quantidade positivas", id: "1" },
                    { name: "Mostrar somente quantidades diferentes de zero", id: "2" }
                ]
        var statusStore = new Memory({
            data: dados
        });
        dijit.byId('tipoRelatorio').store = statusStore;
        dijit.byId('tipoRelatorio').set("value", 1);
    }
    catch (e) {
        postGerarLog(e);
    }
};

function sugereDataCorrente() {
    dijit.byId("dtBase")._onChangeActive = false;

    dojo.xhr.post({
        url: Endereco() + "/util/PostDataHoraCorrente",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try{
            var dataCorrente = jQuery.parseJSON(data).retorno;
            var dataSugerida = dataCorrente.dataPortugues.split(" ");
            if (dataSugerida.length > 0) {
                var date = dojo.date.locale.parse(dataSugerida[0], { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                //Data Inicial: Um mês antes à data do dia
                let d = new Date(date.getFullYear(), date.getMonth(), 01);
                d.setDate(d.getDate() - 1);
                dijit.byId('dtBase').attr("value", d);
                dojo.byId('dtBaseOld').value = dojo.byId('dtBase').value;
                dijit.byId("dtBase")._onChangeActive = true;
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

//#endregion

//#region Monta relatório de estoque
function montarMetodosEstoque() {
    require([
        "dojo/ready",
        "dojo/_base/array",
        "dojo/_base/xhr",
        "dojo/store/Memory",
        "dojo/on",
        "dijit/form/Button",
        "dijit/form/FilteringSelect",
        "dojo/data/ObjectStore",
        "dojo/date",
        "dojo/data/ItemFileReadStore"
    ], function (ready, array, xhr, Memory, on, Button, FilteringSelect, ObjectStore, date, ItemFileReadStore) {
        ready(function () {
            try {
                montarTipoRelatorio(Memory, on);
                sugereDataCorrente();


                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        emitirRelatorio();
                    }
                }, "relatorio");

                var pesItem = document.getElementById('pesItem');
                if (hasValue(pesItem)) {
                    pesItem.parentNode.style.minWidth = '18px';
                    pesItem.parentNode.style.width = '18px';
                }

                dijit.byId('dtBase').on("change", function (e) {
                    try{
                        if (e == null) {
                            var data = dojo.date.locale.parse(dojo.byId("dtBaseOld").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' })
                            dijit.byId('dtBase').set('value', data);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                loadTipoItem('cbTipoItem', ItemFileReadStore);
                dijit.byId('ckEscola').set('checked', eval(MasterGeral()));
                dijit.byId('ckEscola').set('disabled', !eval(MasterGeral()));

            }
            catch (e) {
                postGerarLog(e);
            }
        });
        adicionarAtalhoPesquisa(['pesRelatorio', 'tipoRelatorio', 'dtBase'], 'relatorio', ready);
    });
}
//#endregion

//#region  setDefaultAnoMes  - abrirItemFK -  pesquisarItemEstoqueFK - retornarItemFK - estoqueRpt -  emitirRelatorio
function setoptions(item, option) {
    option.selected = true;
    require(["dojo/on"], function (on) {
        if (option.value >= 0) {
            on(item, "click", function (e) {
                var id = item.parent.id.split("_");
                var checkedMultiSelect = dijit.byId(id[0]);

                var contSelected = 0;
                for (var i = 0; i < checkedMultiSelect.options.length; i++) {
                    if (checkedMultiSelect.options[i].selected == true) {
                        contSelected++;
                    }
                }

                if (option.value == 0) {
                    for (var i = 0; i < checkedMultiSelect.options.length; i++) {
                        checkedMultiSelect.options[i].selected = option.selected;
                    }
                } else {
                    if (checkedMultiSelect.options.length - 1 == contSelected &&
                        checkedMultiSelect.options[0].selected == false) {
                        for (var i = 0; i < checkedMultiSelect.options.length; i++) {
                            checkedMultiSelect.options[i].selected = true;
                        }
                    } else {
                        checkedMultiSelect.options[0].selected = false;
                    }
                }
            });
        }
    });
}

function loadTipoItem(cbTipoItem, ItemFileReadStore) {

    try {
        dojo.xhr.get({
            url: Endereco() + "/api/financeiro/getalltipoitem?tipoMovimento=0",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            var produtos = JSON.parse(data).retorno;
            setProduto(produtos, "cbTipoItem");
        },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
    } catch (e) {

    }
};

function setProduto(produtos, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function (Memory, Array) {
            try {
                var dados = [];
                $.each(produtos, function (index, val) {
                    dados.push({
                        "name": val.dc_tipo_item,
                        "id": val.cd_tipo_item
                    });

                });

                var w = dijit.byId(field);
                //Adiciona a opção todos no checkedmultiselect
                dados.unshift({
                    "name": "Todos",
                    "id": 0
                });
                var storeProduto = new dojo.data.ItemFileReadStore({
                    data: {
                        identifier: "id",
                        label: "name",
                        items: dados
                    }
                });
                if (produtos.length > 0) {
                    w.setStore(storeProduto, []);
                    setMenssageMultiSelect(TPITEM, field);
                    dijit.byId(field).on("change",
                        function (e) {
                            setMenssageMultiSelect(TPITEM, field, true, 60);
                        });
                };
            } catch (e) {

            }
        });
};

function estoqueRpt() {
    try{
        this.isSemColunaC = dijit.byId('ckEscola').checked; //Aproveitando o Booleano existente em UI para testar a escola
        this.tipoRpt = dijit.byId('tipoRelatorio').get('value'); 
        this.dtaInicio = dojo.date.locale.parse(dojo.byId("dtBase").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        this.dc_item = montarparametrosmulti('cbTipoItem');
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarparametrosmulti(componente) {
    var produtos = "";
    if (!hasValue(dijit.byId(componente).value) || dijit.byId(componente).value.length <= 0) {
        produtos = "0";
    } else {

        if (dijit.byId(componente).value[0] == "0") {
            produtos = "0"
        } else {
            for (var i = 0; i < dijit.byId(componente).value.length; i++) {
                if (produtos == "") {
                    produtos = dijit.byId(componente).value[i];
                } else {
                    produtos = produtos + "|" + dijit.byId(componente).value[i];
                }
            }
        }
    }

    return produtos;
}

function emitirRelatorio() {
    try{
        if (!dijit.byId("formRptEstoque").validate())
            return false;
        if (!eval(MasterGeral()) && dijit.byId('ckEscola').checked) {
            caixaDialogo(DIALOGO_AVISO, 'A opção de todas as escolas somente está liberada para usuários Master Geral.', null);
            dijit.byId('ckEscola').set('checked', false);
            return false;
        }

        var estoqueUI = new estoqueRpt();

        require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
            xhr.post(Endereco() + "/api/financeiro/getUrlRelatorioInventario", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(estoqueUI)
            }).then(function (rptEstoque) {
                try{
                    if (!hasValue(rptEstoque.erro)) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioInventario?' + rptEstoque, '1024px', '750px', 'popRelatorio');
                    } else
                        apresentaMensagem('apresentadorMensagem', rptEstoque);
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error.response.data);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion