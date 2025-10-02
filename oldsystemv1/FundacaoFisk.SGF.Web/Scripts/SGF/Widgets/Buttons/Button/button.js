define(["dijit/form/Button", "dojo/query", "dojo/_base/array"],
    function (Button, query, array) {
       

        function createButton(options) {
            var el = {
                elementCreate: {}
                //, elementTemplateCreate: {}
            }

           // el.elementTemplateCreate = renderDomButton(options);
            el.elementCreate = createDomButton(options);

            return el;
        }


        /**
         * CreateButton     
         * @param {(JSON)options} options 
         * @returns {(BtnElement)el} 
         */
        function createDomButton(options) {

           var el = new Button({
                label: (options.label || ''),
                iconClass: (options.iconClass || ''),
                type: (options.btnType || ''),
                disabled: (options.disabled || false),
                onClick: options.callback
            }, options.name);

            if (options.size) {
                setarTamanho(options.name, options.size); 
            }
            return el;
        }

        function setarTamanho(id, tamanho) {
            setTamanhoBotao(document.getElementById(id), tamanho);
        }

        function setTamanhoBotao(btn, px) {
            if (hasValue(btn)) {
                btn.parentNode.style.minWidth = px;
                btn.parentNode.style.width = px;
            }
        }

        return {
            createButton: createButton
            

        };

    }
);