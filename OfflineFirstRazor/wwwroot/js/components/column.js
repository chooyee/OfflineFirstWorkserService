class Column extends Base
{    
    constructor(jsonConfig) {   
        super();     
        this.group = "Basic"
        this.version = "v1";
        this.name = "Column";
        if (jsonConfig!== void 0)        
          this.config = JSON.parse(jsonConfig);        
        else
          this.config =  {};       
     
        let config = this.config;
		if (config.columns === void 0) {config.columns = [];}
        if (config.propertyName === void 0) { config.propertyName = 'Column'; }
        if (config.type === void 0) { config.type = 'Column'; }
        if (config.numOfColumns === void 0) { config.numOfColumns = 3; }
        if (config.formBuilderMode === void 0) { config.formBuilderMode = false; }
        
    }

    getEditFormJson = () =>{
      return {
        "label": "Column Component Edit Form",
        "tabs": [
           {"tabLabel": "Display",
             "tabId":"home",
            "components": [ 
              {
                "propertyName":"position",
                "label": "Position",
                "description": "Set the position of the label", 
                "options":[
					{"left":"left"},
					{"center":"center"},
					{"right":"right"}
				],
                "value": this.config.position,           
                "type": "Select"
              },
			  {
                "propertyName":"numOfColumns",
                "label": "Number of Columns",
                "description": "Set number of columns", 
                "options":["1","2","3","4","5","6","7","8","9","10"],
                "value": this.config.numOfColumns,           
                "type": "Select"
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
              }
            ]
          }
        ]
      };
    }
    
    renderElement = (config, elementId)=>{
      //propertyName, elementId, prefix, suffix, placeholder, letterCase, rows, required
     
    
      //===================================================================================
        //render Input Group
        //===================================================================================
       
        const divInputGroup = this.createElement("div", {"class":"d-flex"});  

		if (config.position ==='center')
        {
          divInputGroup.classList.add("justify-content-center", "align-items-center")
        }
        else if (config.position ==='right')
        {
          divInputGroup.classList.add("justify-content-end", "align-items-right")
        }

        const inputEl = this.renderRawElement(config, elementId);
        
        divInputGroup.appendChild(inputEl);

        const divElement = this.createElement("div", {"ref":"element"});
        divElement.appendChild(divInputGroup);


        return divElement;
        //===================================================================================
        //end render Input Group
        //===================================================================================
    }

    renderRawElement = (config, elementId)=>{
		const propertyName = config.propertyName;
		const componentId = `${this.name}-${elementId}`;
		const container =  this.createElement("div", {"data-property":propertyName,"id":componentId, "class":"container", "ref":"input"});
		
		if (!this.isNullOrEmpty(config.propertyId))
		{
			container.setAttribute("data-id", config.propertyId);
		}
      
        const row =  this.createElement("div", {"id":`row-${elementId}-0`,"data-parent":componentId, "class":"row", "ref":"row"});

		config.columns = [];
		for (let i =0;i<config.numOfColumns;i++)
		{
			const colContainerId = `col-${elementId}-${i}`;
			const col = this.createElement("div", {"class":"col form-column","data-parent":componentId, "ref":"col"});
			const colDropContainer = this.createElement("div", {"id":colContainerId, "data-parent":componentId, "class":"col-drop-container builder-components","ref":"container", "data-col":i});
			//<div data-position="0" data-noattach="true" role="alert" style="text-align:center;" class="builder-component drag-and-drop-alert alert alert-info no-drop">
			//Drag and Drop a form component
			//</div>
			const div = this.createElement("div",{"data-position":"0", "data-noattach":"true", "role":"alert", "style":"text-align:center;", "class":"builder-component drag-and-drop-alert alert alert-info no-drop"})
			div.innerHTML = "Drag and Drop a form component";
			colDropContainer.appendChild(div);
			col.appendChild(colDropContainer);
			row.appendChild(col);

			config.columns.push({"components":[]});
		}
		
		container.appendChild(row);

		this.dropZonEventHandler(container);
      	return container;
    }

	dropZonEventHandler(container){
		const dropzones = container.querySelectorAll(".col-drop-container");
		dropzones.forEach((zone) => {
			zone.addEventListener("dragover", (e) => {
			  dropZoneDragoverEventHandler(zone, e);
			});
	
			zone.addEventListener("dragenter", (e) => { });
	
			zone.addEventListener("dragleave", (e) => {  
			  dropZoneDragleaveEventHandler(zone, e);
			});
	
			zone.addEventListener("drop", (e) => {
			  console.log("drop");
			  //curZone = zone;
			  elementDropped(zone, e);  
			 
			});
	
		});
	}

}
