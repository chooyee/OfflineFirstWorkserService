class Checkbox extends Base
{    
    constructor(jsonConfig) {   
        super();     
        this.group = "Basic"
        this.name = "Checkbox";
        this.version = "v1";
        if (jsonConfig!== void 0)        
          this.config = JSON.parse(jsonConfig);        
        else
          this.config =  {};       
     
        let config = this.config;
        if (config.propertyName === void 0) { config.propertyName = 'Checkbox'; }
        if (config.type === void 0) { config.type = 'Checkbox'; }
        if (config.label === void 0) { config.label = 'My Checkbox'; }
        if (config.labelPosition === void 0) { config.labelPosition = 'Left-Left'; }
        if (config.labelWidth === void 0) { config.labelWidth = 30; }
        if (config.labelMargin === void 0) { config.labelMargin = 3; }
        if (config.value === void 0) { config.value = ""; }
       
        if (config.formBuilderMode === void 0) { config.formBuilderMode = false; }
        
    }

    getEditFormJson = () =>{
      return {
        "label": "Checkbox Component Edit Form",
        "tabs": [
           {"tabLabel": "Display",
             "tabId":"home",
            "components": [
              {
                "propertyName":"label",
                "label": "Label",
                "labelPosition":"Left-Left",
                "description": "Enter the label the checkbox",     
                "validate": {
                  "mandatory": true,
                  "lengthCheck":{"min":0, "max":0}
                },
                "value":this.config.label,           
                "type": "Textbox"
              }
            ]
          },
          {
            "tabLabel": "Data",
            "tabId":"Data",
            "components": [
              {
                "propertyName":"value",
                "label": "Checked by default",
                "type": "Checkbox",
                "value":this.config.value
              }
            ]
          }
        ]
      };
      
    }

    renderDomElement = ()=>{
      let config = this.config;

      const elementId = this.elementId;
      //Main div component
      let div = this.createElement('div', {"id":elementId,"tabindex":"-1","ref":"dragComponent", "class":"builder-component"});

       //Edit component
      //div.appendChild(this.getBuilderEditComponent());
      if (config.formBuilderMode){
        div.appendChild(this.renderBuilderEditComponent());
      }

      //div component
      //<div ref="component" class="mb-2 formio-form-group has-feedback formio-component formio-component-textfield formio-component-textField limau, ice" id="enuyfr7">
      let componentDiv = this.createElement('div',{"id": "component-" + elementId,"ref":"component", "class":"mb-2"});

      div.appendChild(componentDiv);

      //label position
      //const label = this.renderLabel(elementId, config.label);
      const mainElement = this.renderElement(config, elementId);
      if (this.isPropExists(config, "tooltip"))
      {
        label.appendChild(this.renderTooltip(config.tooltip));
      }
      
     
      componentDiv.appendChild(mainElement);
      return div;
    }  


    renderElement = (config, elementId)=>{
        //propertyName, elementId, placeholder, options, required
       
        const desc = config.description;
        //===================================================================================
        //render Input Group
        //===================================================================================
       
        const divInputGroup = this.createElement("div", {"class":"form-check"});       

        const inputEl = this.renderRawElement(config, elementId);        
        divInputGroup.appendChild(inputEl);

        const chkLabel = this.createElement("label", {"class":"form-check-label", "for":`${this.name}-${elementId}`});
        const span = this.createElement("span");
        span.innerHTML = config.label;
        chkLabel.appendChild(span);
        divInputGroup.appendChild(chkLabel);

        const divElement = this.createElement("div", {"ref":"element"});
        divElement.appendChild(divInputGroup);

        if (!this.isNullOrEmpty(desc))
        {
          //<div class="form-text text-muted" id="d-egxfg08-checkbox">testew er ewrewr ewrewr ewre w</div>
          const divDesc = this.createElement("div", {"class":"form-text text-muted"});
          divDesc.innerHTML = desc;
          divElement.appendChild(divDesc);
        }

        return divElement;
        //===================================================================================
        //end render Input Group
        //===================================================================================
    }

    renderRawElement = (config, elementId)=>{
      const propertyName = config.propertyName;
      const defaultValue = config.value;
      const inputEl = this.createElement("input", {"data-property":propertyName,"id":`${this.name}-${elementId}`, "class":"form-check-input", "type":"checkbox", "ref":"input"});
      if (!this.isNullOrEmpty(config.propertyId))
      {
        inputEl.setAttribute("data-id", config.propertyId);
      }

      if (!this.isNullOrEmpty(defaultValue)) {this.setCheckboxState(inputEl, defaultValue)}
      return inputEl;
    }

    setCheckboxState(checkboxElement, checked) {
      
      if (checkboxElement) {
        checkboxElement.checked = checked;
      }
    }
}
