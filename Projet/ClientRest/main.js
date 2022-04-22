function findPath() {
    let origin = formatAddress(document.getElementById("fromAddress").value);
    let dest = formatAddress(document.getElementById("toAddress").value);
    console.log(origin);
    console.log(dest);
    let params = ["origin=" + origin , "dest=" + dest ];
    let request = formatRequest(params);
    console.log(request);

    let caller = new XMLHttpRequest();
    caller.open("GET", request, true);
    caller.setRequestHeader ("Accept", "application/json");
    caller.onload = retrievePath;
    caller.send();
}

function formatAddress(add) {
    let elements = add.split(' ');
    let address = elements[0];
    for(let i=1; i<elements.length; i++) {
        address += "+" + elements[i];
    }
    return address;
}

function formatRequest(params) {
    let fullUrl = "http://localhost:8733/Design_Time_Addresses/RoutingServer/ServiceRest/Path";
    if (params) {
        fullUrl += "?" + params.join("&");
    }
    return fullUrl;
}

function retrievePath() {
    // First of all, check that the call went through OK:
    if (this.status !== 200) {
        console.log("Contracts not retrieved. Check the error in the Network or Console tab.");
    }
    else {
        let responseObject = JSON.parse(this.responseText);
        console.log(responseObject);
        fillDirections(responseObject);
        drawPathOnMap(responseObject);
    }
}

var directionsList = [];
function removePrevDirections() {
    let parent = document.getElementById("directions");
    directionsList.forEach(e => parent.removeChild(e));
    directionsList = [];
}

function getTitle(index,len) {
    if (len===1) {
        return "Destination too close to take the bike ! Let's walk !"
    }
    else {
        if(index===0) {
            return "Walk to the bike station"
        }
        else if (index===1) {
            return "Take a bike and ride to the finish station"
        }
        else {
            return "Leave the bike and walk to your destination !"
        }
    }
}

function formatDistance(distance) {
   if (distance >= 1000) {
       return Math.round(distance/10)/100 + "km";
   }
   return Math.round(distance*100)/100 + "m";
}

function formatTime(time) {
    let hours = Math.floor(time / 3600);
    let minutes = Math.floor((time - (hours * 3600)) / 60);
    let seconds = time - (hours * 3600) - (minutes * 60);

    if (time>=3600) {
        return hours + "h" + minutes + "min" ;
    }
    if (time >= 60) {
        return minutes + "min";
    }
    return seconds + "s";
}

function getInfos(result) {
    let distance = formatDistance(result['distance']);
    let duration = formatTime(result['duration']);
    return " - " + distance +  " - " + duration;
}

function fillDirections(requestResult) {
    removePrevDirections();
    let directionsdiv = document.getElementById("directions");
    let paths = requestResult['GetPathResult'];
    for(let i=0; i<paths.length; i++) {
        let directionssubdiv = document.createElement("DIV");
        directionssubdiv.className = "step";
        directionsList.push(directionssubdiv);
        let title = document.createElement("H4");
        title.className = "title";
        directionssubdiv.appendChild(title);
        title.innerHTML = getTitle(i,paths.length) + getInfos(paths[i]['features'][0]['properties']['segments'][0]) ;
        let stepsDiv = document.createElement("Div");
        stepsDiv.className = "steps"
        directionssubdiv.appendChild(stepsDiv);
        let steps = paths[i]['features'][0]['properties']['segments'][0]['steps'];
        for(let j=0; j<steps.length; j++) {
            let direction = document.createElement("DIV");
            direction.className = "direction"
            let result = steps[j];
            direction.innerHTML = result['instruction'] + " during " + formatTime(result['duration']) + " and " + formatDistance(result['distance'])+ ".";
            stepsDiv.appendChild(direction);
        }
        directionsdiv.appendChild(directionssubdiv);
        if(paths.length===1) {
            directionssubdiv.style.height = "100%";
            title.style.height = "3vh";
            stepsDiv.style.height = "90%";
        }
    }
}
var map;
var prevLayers = [];
var newMap = true;

var colors = ['#9a66bd','#2a58a8','#d11b33'];
function drawPathOnMap(requestResult) {
    newMap = true;
    let result = requestResult['GetPathResult'];
    let first = result[0]['features'][0]['geometry']['coordinates'][0];
    let last = result[result.length-1]['features'][0]['geometry']['coordinates'][result[result.length-1]['features'][0]['geometry']['coordinates'].length-1];
    console.log(first);
    console.log(last);
    for(let i=0; i<result.length; i++) {
        let path = result[i]['features'][0]['geometry']['coordinates'];
        drawLines(path,colors[i]);
    }
    let coord = [first[0],first[1]]
    //let zoom = Math.abs(Math.max(first[1]-first[0],last[1]-last[0]))/8;
    let zoom = 8;
    console.log(zoom);
    let view = new ol.View({
        center: ol.proj.fromLonLat(coord),
        zoom: zoom // You can adjust the default zoom.
    })
    map.setView(view);
}

function createMap() {
    map = new ol.Map({
        target: 'osm', // <-- This is the id of the div in which the map will be built.
        layers: [
            new ol.layer.Tile({
                source: new ol.source.OSM()
            })
        ],
        view: new ol.View({
            center: ol.proj.fromLonLat([7.0985774, 43.6365619]), // <-- Those are the GPS coordinates to center the map to.
            zoom: 10 // You can adjust the default zoom.
        })
    });
}

function drawLines(path,color) {
    // Create an array containing the GPS positions you want to draw
    var lineString = new ol.geom.LineString(path);

// Transform to EPSG:3857
    lineString.transform('EPSG:4326', 'EPSG:3857');

// Create the feature
    var feature = new ol.Feature({
        geometry: lineString,
        name: 'Line'
    });

// Configure the style of the line
    var lineStyle = new ol.style.Style({
        stroke: new ol.style.Stroke({
            color: color,
            width: 5
        })
    });

    var source = new ol.source.Vector({
        features: [feature]
    });

    var vector = new ol.layer.Vector({
        source: source,
        style: [lineStyle]
    });

    if (newMap) {
        prevLayers.forEach(l => map.removeLayer(l));
        newMap = false;
        prevLayers = [];
    }
    map.addLayer(vector);
    prevLayers.push(vector);
}
