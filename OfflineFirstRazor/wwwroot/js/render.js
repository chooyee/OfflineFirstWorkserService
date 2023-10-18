const formJson = {
	"element-id": "drop-container",
	"element-group": "default",
	"element-version": "v1",
	"element-type": "container",
	"element-parent": "",
	"components": [
		{
			"position": "left",
			"numOfColumns": "2",
			"propertyName": "Column",
			"formBuilderMode": true,
			"columns": [
				{
					"components": [
						{
							"headerType": "h1",
							"position": "left",
							"value": "",
							"letterCase": "none",
							"description": "",
							"customCss": "",
							"propertyName": "",
							"formBuilderMode": true,
							"type": "Header",
							"label": "",
							"labelWidth": 30,
							"labelMargin": 3,
							"leftterCase": "none",
							"visibility": {
								"simple": {
									"target": "",
									"value": ""
								}
							},
							"element-id": "component-Uti1XI89N",
							"element-group": "Basic",
							"element-version": "v1",
							"element-type": "Header",
							"element-parent": "col-jAN3SMFcd-0"
						}
					]
				},
				{
					"components": []
				}
			],
			"type": "Column",
			"element-id": "component-jAN3SMFcd",
			"element-group": "Basic",
			"element-version": "v1",
			"element-type": "Column",
			"element-parent": "drop-container"
		},
		{
			"label": "My Number box",
			"labelPosition": "Left-Left",
			"description": "",
			"placeholder": "",
			"prefix": "",
			"suffix": "",
			"customCss": "",
			"validation": {
				"lengthCheck": {
					"min": "0",
					"max": "0"
				}
			},
			"propertyName": "",
			"value": "",
			"pattern": "/[^0-9]/g",
			"formBuilderMode": true,
			"type": "Numberbox",
			"labelWidth": 30,
			"labelMargin": 3,
			"element-id": "component-j6SrF6Vyb",
			"element-group": "Basic",
			"element-version": "v1",
			"element-type": "Numberbox",
			"element-parent": "drop-container"
		}
	]
};

const renderForm = (container, formJson) => {

    if (formJson["element-type"] == 'container') {
        formJson.components.forEach(component => {
           
            component.formBuilderMode = false;
            const el = createInstance(component.type, JSON.stringify(component));
            const domComponent = el.renderDomElement();
            container.appendChild(domComponent);

            if (component["element-type"] === 'Column') {
                //loop Columns array
                for (let i = 0; i < component["columns"].length; i++) {

					//Loop components array in columns
					
                    component["columns"][i]["components"].foreach(colCom => {
                        //getColumnId
                        let columnId = `col-${el.elementId}-${i}`;
                        let columnContainer = domComponent.getElementById(columnId);
                        renderForm(columnContainer, colCom);
                    });
                }
                
            }
        });
    }
    
    return container;
}

const createInstance = (classNameString, json) =>{
    var obj = new (eval(classNameString))(json);
    return obj
};

const createElement = (elementType, attrs = {})=>
{
	const el = document.createElement(elementType);
	this.appendAttr(el, attrs);
	return el;
}

const appendAttr = (el, attrs = {})=>
{
	for (const [k, v] of Object.entries(attrs)) {
		el.setAttribute(k, v);
	}
	return el;
}