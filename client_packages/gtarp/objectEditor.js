/**
 *  Grand Theft Auto Roleplay GameMode - GTARPES
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Grand Theft Auto: Roleplay [gta-rp.es]
 *  
 *  objectEditor.js - Object editor
 */

//Editor constants
const OBJECT_EDITOR_DEFAULT_VELOCITY = 0.1;
const OBJECT_EDITOR_MIN_VELOCITY = 0.01;
const OBJECT_EDITOR_MAX_VELOCITY = 1.0;
const OBJECT_EDITOR_COMPONENTS = 6;
const OBJECT_EDITOR_MAX_OFFSET = 10.0;

var objectEditorVisible = false;
var objectEditorTitle = '';
var objectEditorMode = 0;
var objectEditorVelocity = 0.0;
var objectEditorComponent = 0;
var objectEditorOffset;
var objectEditorRotation;
var objectEditorPosition;

//Editor mode names
var modeName = [
    'Objeto enganchado',
    'Mueble',
    'Barricada/Señalización'
];

//Editor components
var component = {
    XOffset: 0,
    YOffset: 1,
    ZOffset: 2,
    XRotation: 3,
    YRotation: 4,
    ZRotation: 5
};

//Editor component names
var componentName = [
    'Desplazamiento X',
    'Desplazamiento Y',
    'Desplazamiento Z',
    'Rotación X',
    'Rotación Y',
    'Rotación Z'
];

API.onServerEventTrigger.connect(function (name, args) {
    
    if(name === 'object_editor_start') {
        objectEditorVisible = args[0];
        objectEditorTitle = args[1];
        objectEditorMode = args[2];
        objectEditorVelocity = OBJECT_EDITOR_DEFAULT_VELOCITY;
        objectEditorComponent = args[3];
        objectEditorOffset = args[4];
        objectEditorRotation = args[5];
        objectEditorPosition = args[6];
    }
    
});


//Key down eventhandler (repeats every tick if pressed)
API.onKeyDown.connect(function (sender, args) {
    if (objectEditorVisible) {
        switch(args.KeyCode) {
            case Keys.Right: {
                increaseComponent();
                break;
            }
            case Keys.Left: {
                decreaseComponent();
                break;
            }
            case Keys.Up: {
                switchEditingComponent();
                break;
            }
            case Keys.Down: {
                changeEditingVelocity();
                break;
            }
            case Keys.Enter: {
                objectEditorVisible = false;
                API.triggerServerEvent('key_enter', objectEditorOffset, objectEditorRotation, objectEditorPosition);
                break;
            }
            case Keys.Back: {
                objectEditorVisible = false;
                API.triggerServerEvent('key_back');
                break;
            }
        }
    }
});

// Renderer
API.onUpdate.connect(function () {
    //Object editor UI
    if (objectEditorVisible) {

        var title = objectEditorTitle;
        var mode = modeName[objectEditorMode];
        var component = componentName[objectEditorComponent];
        var velocity = objectEditorVelocity.toString();
        var position = objectEditorOffset;
        var rotation = objectEditorRotation;

        API.drawRectangle(0.0, 600.0, 450.0, 250.0, 0, 0, 0, 150);

        API.drawText(title, 10.0, 620.0, 0.4, 255, 191, 0, 255, 0, 0, false, true, 0);
        API.drawText('Modo: ', 10.0, 660.0, 0.3, 255, 191, 0, 255, 0, 0, false, true, 0);
        API.drawText(mode, 70.0, 660.0, 0.3, 255, 255, 255, 255, 0, 0, false, true, 0);
        API.drawText('Componente: ', 10.0, 680.0, 0.3, 255, 191, 0, 255, 0, 0, false, true, 0);
        API.drawText(component, 130.0, 680.0, 0.3, 255, 255, 255, 255, 0, 0, false, true, 0);
        API.drawText('Velocidad: ', 10.0, 700.0, 0.3, 255, 191, 0, 255, 0, 0, false, true, 0);
        API.drawText(velocity, 100.0, 700.0, 0.3, 255, 255, 255, 255, 0, 0, false, true, 0);
        API.drawText('Posición: ', 10.0, 720.0, 0.3, 255, 191, 0, 255, 0, 0, false, true, 0);
        API.drawText(position.X.toFixed(2) + ' , ' + position.Y.toFixed(2) + ' , ' + position.Z.toFixed(2), 90.0, 720.0, 0.3, 255, 255, 255, 255, 0, 0, false, true, 0);
        API.drawText('Rotación: ', 10.0, 740.0, 0.3, 255, 191, 0, 255, 0, 0, false, true, 0);
        API.drawText(rotation.X.toFixed(2) + ' , ' + rotation.Y.toFixed(2) + ' , ' + rotation.Z.toFixed(2), 90.0, 740.0, 0.3, 255, 255, 255, 255, 0, 0, false, true, 0);
        API.drawText('Cambiar componente: flecha arriba | Cambiar velocidad: flecha abajo', 10.0, 780.0, 0.2, 255, 191, 0, 255, 0, 0, false, true, 0);
        API.drawText('Editar: flechas izquierda y derecha | Guardar: Enter | Cancelar: Retroceso', 10.0, 800.0, 0.2, 255, 191, 0, 255, 0, 0, false, true, 0);

    }
});

/**
* switchEditingComponent()
* Switches the editing component
*/
function switchEditingComponent() {
    objectEditorComponent++;

    if (objectEditorComponent === OBJECT_EDITOR_COMPONENTS)
    {
        objectEditorComponent = 0;
    }
}

/**
* changeEditingVelocity()
* Changes the editing velocity
*/
function changeEditingVelocity() {
    switch (objectEditorVelocity)
    {
        case OBJECT_EDITOR_DEFAULT_VELOCITY:
            {
                objectEditorVelocity = OBJECT_EDITOR_MAX_VELOCITY;
                break;
            }
        case OBJECT_EDITOR_MAX_VELOCITY:
            {
                objectEditorVelocity = OBJECT_EDITOR_MIN_VELOCITY;
                break;
            }
        case OBJECT_EDITOR_MIN_VELOCITY:
            {
                objectEditorVelocity = OBJECT_EDITOR_DEFAULT_VELOCITY;
                break;
            }
    }
}
/**
* increaseComponent()
* Increase the selected component
*/
function increaseComponent()
{
    switch (objectEditorComponent) {
        case component.XOffset: {
            objectEditorPosition.X += objectEditorVelocity;
            objectEditorOffset.X += objectEditorVelocity;
            break;
        }
        case component.YOffset: {
            objectEditorPosition.Y += objectEditorVelocity;
            objectEditorOffset.Y += objectEditorVelocity;
            break;
        }
        case component.ZOffset: {
            objectEditorPosition.Z += objectEditorVelocity;
            objectEditorOffset.Z += objectEditorVelocity;
            break;
        }
        case component.XRotation: {
            objectEditorRotation.X += objectEditorVelocity;
            break;
        }
        case component.YRotation: {
            objectEditorRotation.Y += objectEditorVelocity;
            break;
        }
        case component.ZRotation: {
            objectEditorRotation.Z += objectEditorVelocity;
            break;
        }
    }

    API.triggerServerEvent('object_editor_update', objectEditorOffset, objectEditorRotation, objectEditorPosition);
}

/**
* decreaseComponent()
* Decrease the selected component
*/
function decreaseComponent() {
    switch (objectEditorComponent) {
        case component.XOffset: {
            objectEditorPosition.X -= objectEditorVelocity;
            objectEditorOffset.X -= objectEditorVelocity;
            break;
        }
        case component.YOffset: {
            objectEditorPosition.Y -= objectEditorVelocity;
            objectEditorOffset.Y -= objectEditorVelocity;
            break;
        }
        case component.ZOffset: {
            objectEditorPosition.Z -= objectEditorVelocity;
            objectEditorOffset.Z -= objectEditorVelocity;
            break;
        }
        case component.XRotation: {
            objectEditorRotation.X -= objectEditorVelocity;
            break;
        }
        case component.YRotation: {
            objectEditorRotation.Y -= objectEditorVelocity;
            break;
        }
        case component.ZRotation: {
            objectEditorRotation.Z -= objectEditorVelocity;
            break;
        }
    }

    API.triggerServerEvent('object_editor_update', objectEditorOffset, objectEditorRotation, objectEditorPosition);
}
