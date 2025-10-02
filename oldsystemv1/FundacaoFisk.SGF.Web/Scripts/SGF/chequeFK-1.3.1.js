var TITULO = null;

function showViewCheque(titulo,mostrar) {
    if(mostrar) dijit.byId("proChequeFK").show();
    this.TITULO = titulo;

    if (hasValue(this.TITULO.Cheque)) {
        dojo.byId("cd_cheque").value = this.TITULO.Cheque.cd_cheque;
        dijit.byId("emissorName").set('value', this.TITULO.Cheque.no_emitente_cheque);
        dijit.byId("agenciaCh").set('value', this.TITULO.Cheque.no_agencia_cheque);
        dijit.byId("nroAgenciaCheque").set('value', this.TITULO.Cheque.nm_agencia_cheque);
        dijit.byId("dgAgenciaCheque").set('value', this.TITULO.Cheque.nm_digito_agencia_cheque);
        dijit.byId("nroContaCorrenteCheque").set('value', this.TITULO.Cheque.nm_conta_corrente_cheque);
        dijit.byId("dgContaCorrenteCheque").set('value', this.TITULO.Cheque.nm_digito_cc_cheque);

        //if (hasValue(this.TITULO.Cheque.nm_primeiro_cheque))
        //    dijit.byId("nroCheque").set('value', this.TITULO.Cheque.nm_primeiro_cheque);
        //else
        //    dijit.byId("nroCheque").set('value', this.TITULO.dc_num_documento_titulo);

        dijit.byId("bancosViewCheque").set('value', this.TITULO.Cheque.cd_banco);

        if (hasValue(this.TITULO.Cheque.dt_bom_para)) {
            dijit.byId("nroCheque").set('value', this.TITULO.Cheque.nm_primeiro_cheque);
            dijit.byId("dtBomParaCheque").set('value', this.TITULO.Cheque.dt_bom_para);
        }
        else {
            dijit.byId("nroCheque").set('value', this.TITULO.dc_num_documento_titulo);
            dijit.byId("dtBomParaCheque").set('value', this.TITULO.dt_vcto_titulo);
        }

    } else {
        dijit.byId("cadCheque").reset();
    }
}

function showViewChequeTroca(cheque, Titulo) {
    dijit.byId("proChequeFK").show();


    if (!hasValue(cheque) && hasValue(this.TITULO) && hasValue(this.TITULO.Cheque)) {
	    cheque = this.TITULO.Cheque;
    }

    if (hasValue(cheque)) {
        dojo.byId("cd_cheque").value = cheque.cd_cheque;
        dijit.byId("emissorName").set('value', cheque.no_emitente_cheque);
        dijit.byId("agenciaCh").set('value', cheque.no_agencia_cheque);
        dijit.byId("nroAgenciaCheque").set('value', cheque.nm_agencia_cheque);
        dijit.byId("dgAgenciaCheque").set('value', cheque.nm_digito_agencia_cheque);
        dijit.byId("nroContaCorrenteCheque").set('value', cheque.nm_conta_corrente_cheque);
        dijit.byId("dgContaCorrenteCheque").set('value', cheque.nm_digito_cc_cheque);

        //if (hasValue(cheque.nm_primeiro_cheque))
        //    dijit.byId("nroCheque").set('value', cheque.nm_primeiro_cheque);
        //else
        //    dijit.byId("nroCheque").set('value', cheque.dc_num_documento_titulo);

        dijit.byId("bancosViewCheque").set('value', cheque.cd_banco);

        if (hasValue(cheque.dt_bom_para)) {
            dijit.byId("nroCheque").set('value', cheque.nm_primeiro_cheque);
            dijit.byId("dtBomParaCheque").set('value', cheque.dt_bom_para);
        }
        else {
            dijit.byId("nroCheque").set('value', cheque.dc_num_documento_titulo);
            dijit.byId("dtBomParaCheque").set('value', cheque.dt_vcto_titulo);
        }

    } else {
        dijit.byId("cadCheque").reset();
        dojo.byId("cd_titulo").value = Titulo.cd_titulo;
    }
}

function incluirCheque(nr_coluna) {
    if (!dijit.byId("cadCheque").validate())
        return false;

    if (hasValue(dijit.byId("gridBaixaCad").store.objectStore.data) && !hasValue(this.TITULO)) {
	    var tituloGrid = dijit.byId("gridBaixaCad").store.objectStore.data.filter(function (value, index) {
		    return value.Titulo.cd_titulo == parseInt(dojo.byId("cd_titulo").value);
	    });

	    if (hasValue(tituloGrid)) {
		    this.TITULO = tituloGrid[0].Titulo;
	    }
    }
    

    if (hasValue(this.TITULO)) {
        this.TITULO.Cheque = {
            cd_cheque: dojo.byId("cd_cheque").value,
            no_emitente_cheque: dijit.byId("emissorName").get('value'),
            no_agencia_cheque: dijit.byId("agenciaCh").get('value'),
            nm_agencia_cheque: dijit.byId("nroAgenciaCheque").get('value'),
            nm_digito_agencia_cheque: parseInt(dijit.byId("dgAgenciaCheque").get('value')),
            nm_conta_corrente_cheque: dijit.byId("nroContaCorrenteCheque").get('value'),
            nm_digito_cc_cheque: parseInt(dijit.byId("dgContaCorrenteCheque").get('value')),
            nm_primeiro_cheque: dijit.byId("nroCheque").get('value'),
            cd_banco: dijit.byId("bancosViewCheque").get('value'),
            dt_bom_para: dijit.byId("dtBomParaCheque").get('value'),
            existe_cheque: true
        }
        alterarLabelGridTitulo(nr_coluna);
    }
    dijit.byId("proChequeFK").hide();
}

function alterarLabelGridTitulo() {
    if (hasValue(EVENTO_GRID_CHEQUE && this.TITULO.Cheque))
        EVENTO_GRID_CHEQUE.cellNode.children[0].children[0].innerText = "Alterar";
    else
        EVENTO_GRID_CHEQUE.cellNode.children[0].children[0].innerText = "Adicionar";
}

function loadBancoViewCheque(cd_banco, Titulo, mostrar) {
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getAllBanco",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataBanco) {
        try {
            if (hasValue(dataBanco.retorno)) {
                criarOuCarregarCompFiltering("bancosViewCheque", dataBanco.retorno, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_banco', 'no_banco');
            }
            if (Titulo.id_origem_titulo == 129 && Titulo.cd_tipo_financeiro == 4 && !hasValue(Titulo.Cheque)) {
                loadChequeTrocaFinanceira(cd_banco, Titulo);
            } else {
                showViewCheque(Titulo,mostrar);
            }
            
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemMat', error);
    });
}

function loadChequeTrocaFinanceira(cd_banco, Titulo) {
    dojo.xhr.get({
        url: Endereco() + "/api/escola/loadChequeTrocaFinanceira?cd_titulo="+ Titulo.cd_titulo,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (cheque) {
            try {
                showViewChequeTroca(cheque, Titulo);
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemMat', error);
        });
}


