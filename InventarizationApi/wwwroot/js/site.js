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

    if (type === 'marker') {
    }
    editableLayers.addLayer(layer);
});

function saveWarehouse(url) {
    const layers = [];
    editableLayers.eachLayer(
        function (layer) {
            const geojson = layer.toGeoJSON();
            layers.push(geojson.geometry);
        }
    )

    const name = $('#org_name').val();
    const activity = $('#org_activity').val();

    const data = JSON.stringify({
        "Name": name,
        "ActivityType": activity,
        "GeoObjects": layers
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
    selectedLayers.clearLayers();
    const location = e.latlng;
    const selected = L.tileLayer.wms(`http://localhost:8081/geoserver/inventarization/wms?viewparams=lat:${location.lng};lon:${location.lat}`, {
        layers: 'inventarization:warehouse_object_inter',
        format: 'image/png',
        transparent: true
    });
    selectedLayers.addLayer(selected);
    // selectedLayers.bringToFront();
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
