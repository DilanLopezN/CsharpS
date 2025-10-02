function montarUsuarioSenha() {
    require([
        "dijit/form/Button",
        "dojo/ready"
    ], function (Button, ready) {
        ready(function () {
            try {
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322975', '765px', '771px');
                        });
                }

                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { Salvar(); } }, "salvar");
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function Salvar() {
    try {
        var mensagensWeb = new Array();

        if (!dijit.byId("formularioSenha").validate()) {
            return false;
        }
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            xhr.post(Endereco() + "/Api/UsuarioSenha/PostAlteraUsuarioSenha/", {
                data: ref.toJson({
                    SenhaAtual: dom.byId("SenhaAtual").value,
                    NovaSenha: dom.byId("NovaSenha").value,
                    ConfirmaNovaSenha: dom.byId("ConfirmaNovaSenha").value
                }),
                headers: { "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    if (hasValue(data)) {
                        data = jQuery.parseJSON(data);
                        if (!hasValue(data.erro)) {
                            apresentaMensagem('apresentadorMensagem', data);
                            getLimpar('#formularioSenha');
                        } else
                            apresentaMensagem('apresentadorMensagemFunc', data);
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, jQuery.parseJSON(eval(error.response.data)).MensagensWeb[0].mensagem);
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}