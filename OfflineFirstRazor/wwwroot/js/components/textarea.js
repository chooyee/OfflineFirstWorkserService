class Textarea extends Base
{    
    constructor(jsonConfig) {   
        super();     
        this.group = "Basic"
        this.version = "v1";
        this.name = "Textarea";
        if (jsonConfig!== void 0)        
          this.config = JSON.parse(jsonConfig);        
        else
          this.config =  {};       
     
        let config = this.config;
        if (config.propertyName === void 0) { config.propertyName = 'textarea'; }
        if (config.type === void 0) { config.type = 'Textarea'; }
        if (config.label === void 0) { config.label = 'My Text Area'; }
        if (config.labelPosition === void 0) { config.labelPosition = 'Left-Left'; }
        if (config.labelWidth === void 0) { config.labelWidth = 30; }
        if (config.labelMargin === void 0) { config.labelMargin = 3; }
        if (config.rows === void 0) { config.rows = 3; }
        if (config.letterCase === void 0) { config.letterCase = "none"; }
        if (config.value === void 0) { config.value = ""; }
        if (config.required === void 0) { config.required = false; }
        config.required = config.required=='on'?true:false;
        if (config.minLength === void 0) { config.minLength = 0; }
        if (config.maxLength === void 0) { config.maxLength = 0; }
        if (config.formBuilderMode === void 0) { config.formBuilderMode = false; }
       
        if (config.validation === void 0) {
          config.validation = {}
          config.validation.mandatory = false;

          if (config.validation.lengthCheck === void 0) {
            config.validation.lengthCheck = {};
            config.validation.lengthCheck.min = 0;
            config.validation.lengthCheck.max = 0;
          };
        };
        
    }

    getEditFormJson = () =>{
      return {
        "label": "Text Area Component Edit Form",
        "tabs": [
           {"tabLabel": "Display",
             "tabId":"home",
            "components": [              
              {
                "propertyName":"label",
                "label": "Label",
                "placeholder": "My Text Area label",
                "description": "Enter the label for this text area", 
                "validation":{"mandatory": true, "lengthCheck": {"min": 0, "max":0}},
                "value":this.config.label,           
                "type": "Textbox"
              },
              {
                "propertyName":"labelPosition",
                "label": "Label Position",
                "placeholder": "Label Position",
                "description": "Set the position of the label", 
                "options":[
                  {"Top":"Top"},
                  {"Left-Left":"Left"},
                  {"None":"None"}
                ],
                "value": this.config.labelPosition,           
                "type": "Select"
              },
              {
                "propertyName":"description",
                "label": "Descriptions (Optional)",
                "description": "Description for text area", 
                "type": "Textbox",
                "value": this.config.description
              },
              {
                "propertyName":"rows",
                "label": "Rows",
                "type": "Textbox",
                "value": this.config.rows
              },
              {
                "propertyName":"customCss",
                "label": "Custom CSS",
                "placeholder": "custom css",
                "description": "Set the custom css, for ex: form-control form-item", 
                "type": "Textbox",
                "value": this.config.customCss
              }
            ]
          },
          {
            "tabLabel": "Validation",
            "tabId":"validate",
            "components": [
              {
                "propertyGroup":"validation",
                "propertyType":"mandatory",
                "propertyName":"validation-mandatory",
                "label": "This is mandatory field?",
                "value":this.config.validation.mandatory, 
                "type": "Checkbox"
              },
              {
                "propertyGroup":"validation",
                "propertyType":"lengthCheck",
                "propertyName":"validation-lengthCheck-min",
                "label": "Minimum Length (Optional)",
                "placeholder": "Minimum Length",  
                "type": "Textbox",
                "value": this.config.validation.lengthCheck.min
              },
              {
                "propertyGroup":"validation",
                "propertyType":"lengthCheck",
                "propertyName":"validation-lengthCheck-max",
                "label": "Maximum Length (Optional)",
                "placeholder": "Maximum Length",
                "type": "Textbox",
                "value": this.config.validation.lengthCheck.max
              }
            ]
          },
          {
            "tabLabel": "Data",
            "tabId":"data",
            "components": [
              {
                "propertyName":"propertyName",
                "label": "Data Property",
                "value": this.config.propertyName, 
                "type": "Textbox"
              },
              {
                "propertyName":"value",
                "label": "Default Value",
                "value": this.config.value, 
                "type": "Textarea"
              }
            ]
          }
        ]
      };
    }
    
    renderElement = (config, elementId)=>{
      //propertyName, elementId, prefix, suffix, placeholder, letterCase, rows, required
     
      const desc = config.description;
      //===================================================================================
        //render Input Group
        //===================================================================================
       
        const divInputGroup = this.createElement("div", {"class":"input-group"});       

        const divFormFloat = this.createElement("div", {"class":"form-floating"}); 
		    divInputGroup.appendChild(divFormFloat);
        const inputEl = this.renderRawElement(config, elementId);

        divFormFloat.appendChild(inputEl);
        const label = this.createElement("label",{"for":`${this.name}-${elementId}`});
        label.innerHTML = config.label;
        divFormFloat.appendChild(label);

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
      const val = config.value;
      const rows = config.rows;
      const inputEl =  this.createElement("textarea", {"style":"height:100%","placeholder":config.label,"data-property":propertyName,"id":`${this.name}-${elementId}`, "class":"form-control", "type":"textarea", "rows":rows, "ref":"input"});
      
      if (!this.isNullOrEmpty(config.propertyId))
      {
        inputEl.setAttribute("data-id", config.propertyId);
      }
      
          
      if (this.isPropExists(config,"validation"))
      {
        
        if (this.isPropExists(config.validation, "mandatory"))
        {
          inputEl.classList.add("mandatory");
        }
        if (this.isPropExists(config.validation, "lengthCheck"))
        {
          
          if (typeof Number(config.validation.lengthCheck.min) == 'number'){
            if (+config.validation.lengthCheck.min > 0)
              inputEl.setAttribute("minLength", config.validation.lengthCheck.min);
          }

          if (typeof Number(config.validation.lengthCheck.max) == 'number'){
            
            if (+config.validation.lengthCheck.max > 0){
              inputEl.setAttribute("maxLength", config.validation.lengthCheck.max);
            }
          }
        }

      }
       
      if (!this.isNullOrEmpty(val)) inputEl.innerHTML = val;

      switch (config.letterCase)
      {
        case "upper":
          inputEl.classList.add("text-uppercase");
          break;
        case "lower":
          inputEl.classList.add("text-lowercase");
          break;
        case "capital":
          inputEl.classList.add("text-capitalize");
          break;
      }

      return inputEl;
    }



}
