function initGraphs() {
    $('#addressChart').empty();
    $('#addressLoader').show();
    $.ajax({
        url: "/Graph/GetOrdersByAddress",
        dataType: "json",
        type: "GET",
        success: function (data) {
            $('#addressLoader').hide();
            updateAddressChartData(data);
        }
    });

    $('#productsChart').empty();
    $('#productsLoader').show();
    $.ajax({
        url: "/Graph/GetTopSoldProducts",
        dataType: "json",
        type: "GET",
        success: function (data) {
            $('#productsLoader').hide();
            updateProductsChartData(data);
        }
    });

    $('#paymentChart').empty();
    $('#paymentLoader').show();
    $.ajax({
        url: "/Graph/GetAverageOrderPayementPerMonth",
        dataType: "json",
        type: "GET",
        success: function (data) {
            $('#paymentLoader').hide();
            updatePaymentChartData(data);
        }
    });
};


function updateAddressChartData(data) {
    var width = 400;
    var height = 400;

    var arc = d3.arc()
        .innerRadius(0)
        .outerRadius(Math.min(width, height) / 2 - 1);

    var radius = Math.min(width, height) / 2 * 0.8;
    var arcLabel = d3.arc().innerRadius(radius).outerRadius(radius);
    var pie = d3.pie()
        .sort(null)
        .value(d => d.ordersCount);

    var color = d3.scaleOrdinal()
        .domain(data.map(d => d.address))
        .range(d3.quantize(t => d3.interpolateSpectral(t * 0.8 + 0.1), data.length).reverse());

    const arcs = pie(data);

    const svg = d3.select("#addressChart").append("svg")
        .attr("viewBox", [-width / 2, -height / 2, width, height]);

    svg.append("g")
        .attr("stroke", "white")
        .selectAll("path")
        .data(arcs)
        .join("path")
        .attr("fill", d => color(d.data.address))
        .attr("d", arc)
        .append("title")
        .text(d => `${d.data.address}: ${d.data.ordersCount.toLocaleString()}`);

    svg.append("g")
        .attr("font-family", "sans-serif")
        .attr("font-size", 12)
        .attr("text-anchor", "middle")
        .selectAll("text")
        .data(arcs)
        .join("text")
        .attr("transform", d => `translate(${arcLabel.centroid(d)})`)
        .call(text => text.append("tspan")
            .attr("y", "-0.4em")
            .attr("font-weight", "bold")
            .text(d => d.data.address))
        .call(text => text.filter(d => (d.endAngle - d.startAngle) > 0.25).append("tspan")
            .attr("x", 0)
            .attr("y", "0.7em")
            .attr("fill-opacity", 0.7)
            .text(d => d.data.ordersCount.toLocaleString()));
}


function updateProductsChartData(data) {
    var margin = ({ top: 30, right: 0, bottom: 10, left: 250 });
    const width = 800;
    const barHeight = 30;
    var height = Math.ceil((data.length + 0.1) * barHeight) + margin.top + margin.bottom

    var x = d3.scaleLinear()
        .domain([0, d3.max(data, d => d.ordersCount)])
        .range([margin.left, width - margin.right]);

    var format = x.tickFormat(20, data.format);

    var y = y = d3.scaleBand()
        .domain(d3.range(data.length))
        .rangeRound([margin.top, height - margin.bottom])
        .padding(0.1);

    xAxis = g => g
        .attr("transform", `translate(0,${margin.top})`)
        .call(d3.axisTop(x))
        .call(g => g.select(".domain").remove());

    yAxis = g => g
        .attr("transform", `translate(${margin.left},0)`)
        .call(d3.axisLeft(y).tickFormat(i => data[i].productName));

    const svg = d3.select("#productsChart").append("svg")
        .attr("viewBox", [0, 0, width, height]);

    svg.append("g")
        .attr("fill", "steelblue")
        .selectAll("rect")
        .data(data)
        .join("rect")
        .attr("x", x(0))
        .attr("y", (d, i) => y(i))
        .attr("width", d => x(d.ordersCount) - x(0))
        .attr("height", y.bandwidth());

    svg.append("g")
        .attr("fill", "white")
        .attr("text-anchor", "end")
        .attr("font-family", "sans-serif")
        .attr("font-size", 12)
        .selectAll("text")
        .data(data)
        .join("text")
        .attr("x", d => x(d.ordersCount))
        .attr("y", (d, i) => y(i) + y.bandwidth() / 2)
        .attr("dy", "0.35em")
        .attr("dx", -4)
        .text(d => format(d.ordersCount))
        .call(text => text.filter(d => x(d.ordersCount) - x(0) < 20)
            .attr("dx", +4)
            .attr("fill", "black")
            .attr("text-anchor", "start"));

    svg.append("g")
        .call(xAxis);

    svg.append("g")
        .call(yAxis);
}

function updatePaymentChartData(data) {
    var width = 400;
    var height = 400;

    var arc = d3.arc()
        .innerRadius(0)
        .outerRadius(Math.min(width, height) / 2 - 1);

    var radius = Math.min(width, height) / 2 * 0.8;
    var arcLabel = d3.arc().innerRadius(radius).outerRadius(radius);
    var pie = d3.pie()
        .sort(null)
        .value(d => d.average);

    var color = d3.scaleOrdinal()
        .domain(data.map(d => d.month))
        .range(d3.quantize(t => d3.interpolateSpectral(t * 0.8 + 0.1), data.length).reverse());

    const arcs = pie(data);

    const svg = d3.select("#paymentChart").append("svg")
        .attr("viewBox", [-width / 2, -height / 2, width, height]);

    svg.append("g")
        .attr("stroke", "white")
        .selectAll("path")
        .data(arcs)
        .join("path")
        .attr("fill", d => color(d.data.month))
        .attr("d", arc)
        .append("title")
        .text(d => `${d.data.month}: ${d.data.average.toLocaleString()}`);

    svg.append("g")
        .attr("font-family", "sans-serif")
        .attr("font-size", 12)
        .attr("text-anchor", "middle")
        .selectAll("text")
        .data(arcs)
        .join("text")
        .attr("transform", d => `translate(${arcLabel.centroid(d)})`)
        .call(text => text.append("tspan")
            .attr("y", "-0.4em")
            .attr("font-weight", "bold")
            .text(d => d.data.month))
        .call(text => text.filter(d => (d.endAngle - d.startAngle) > 0.25).append("tspan")
            .attr("x", 0)
            .attr("y", "0.7em")
            .attr("fill-opacity", 0.7)
            .text(d => d.data.average.toLocaleString()));
}
