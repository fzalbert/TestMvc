// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
const map = L.map('map', {
    center: [55.796391, 49.108891], zoom: 13
});

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19, attribution: '© OpenStreetMap'
}).addTo(map);

const editableLayers = new L.FeatureGroup();
const selectedLayers = new L.FeatureGroup();

var selectedObjects = null;

map.addLayer(editableLayers);
map.addLayer(selectedLayers);

const layers = L.tileLayer.wms("http://localhost:8081/geoserver/inventarization/wms", {
    layers: 'inventarization:warehouse_object',
    format: 'image/png',
    transparent: true
});

layers.addTo(map);

map.on('click', mapClicked);

const options = {
    position: 'topright', draw: {
        polygon: {
            allowIntersection: false,
            drawError: {
                color: '#e1e100',
                message: '<strong>Oh snap!<strong> you can\'t draw that!'
            }, shapeOptions: {
                color: '#97009c'
            }
        }, polyline: {
            shapeOptions: {
                color: '#f357a1',
                weight: 10
            }
        }, marker: true,
        rectangle: true,
    }, edit: {
        featureGroup: editableLayers,
        remove: true
    }
};

var drawControl = new L.Control.Draw(options);

map.on('draw:created', function (e) {
    const type = e.layerType, layer = e.layer;
    
    editableLayers.addLayer(layer);
});

function saveWarehouse(url) {
    const layers = [];
    editableLayers.eachLayer(
        function (layer) {
            const geojson = layer.toGeoJSON();
            layers.push(geojson);
        }
    )

    const name = $('#org_name').val();
    const activity = $('#org_activity').val();
    
    const data = JSON.stringify({
        "Type": "FeatureCollection",
        "Features": layers
    });

    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json",
        dataType: "json",
        data: data,
        success: function (result) {
            
            map.on('click', mapClicked);
            removeControl();
            isDraw = false;
            $("#warehouse_container").css("visibility", "collapse");
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert("Status: " + textStatus); alert("Error: " + errorThrown);
        }
    })
}

function mapClicked(e) {
    if(selectedObjects != null) {
        map.removeLayer(selectedObjects);
        selectedObjects = null;
    }
    
    selectedLayers.clearLayers();
    const location = e.latlng;
    
    $.ajax({
        type: "GET",
        url: `http://localhost:5148/api/warehouse?lat=${location.lng}&lon=${location.lat}`,
        contentType: "application/json",
        dataType: "json",
        success: function (result) {
            showSelected(result, e.latlng)
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert("Status: " + textStatus); alert("Error: " + errorThrown);
        }
    })
    
}

function showSelected(items, location){
    
    var geoObjects = [];
    var warehouseInfo = [];
    items.forEach((element) => {
        geoObjects = geoObjects.concat(element.geoObjects)
        warehouseInfo.push(
            {
                name: element.name,
                type: element.activityType
            }
        );
    });
    
    var myStyle = {
        "color": "#ff7800",
        "weight": 5,
        "opacity": 0.65,
        "z-index": 2000
    };
    
    selectedObjects = L.geoJSON(geoObjects, {style: myStyle})
        .addTo(map);
    
    var popupView = "";
    warehouseInfo.forEach((element) => {
        var name = element.name
        var type = element.type;
        popupView += '<p>' + 'Name: ' + name + '\n';
        popupView += 'Type: ' + type + '</p>';
        popupView += '<p></p>';
    });
    selectedObjects.bindPopup(popupView).openPopup();
        
    
    // selectedLayers.addLayer(selected);
}


var isDraw = false;
function drawClicked(){
    if(isDraw){
        map.on('click', mapClicked);
        removeControl();
    }
    else {
        map.off('click', mapClicked);
        addControl()
    }
        
    
    isDraw = !isDraw;
}
function removeControl() {
    editableLayers.clearLayers();
    map.removeControl(drawControl);

    const drawBtn = $("#draw_btn");

    $("#save_btn").css("visibility", "collapse");
    drawBtn.html("Нарисовать");
}

function addControl() {
    map.addControl(drawControl);
    const drawBtn = $("#draw_btn");
    $("#save_btn").css("visibility", "visible");
    drawBtn.html('X');
}

// Write your JavaScript code.
