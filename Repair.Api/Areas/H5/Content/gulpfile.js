var gulp = require("gulp");
var gutil = require("gulp-util");
var combiner = require("stream-combiner2");
var watchPath = require("gulp-watch-path");
var sourcemaps = require("gulp-sourcemaps");
var uglify = require("gulp-uglify");

// 控制输出的颜色变化;
var handleError = function (err) {
    var colors = gutil.colors;
    console.log("\n");
    gutil.log(colors.red("Error!"));
    gutil.log("fileName" + colors.red(err.fileeName));
    gutil.log("lineNumber" + colors.red(err.lineNumber));
    gutil.log("message" + err.message);
    gutil.log("plugin" + colors.yellow(err.plugin));
}

gulp.task("default", function () {
    gutil.log("message");
    gutil.log(gutil.colors.red("error"));
    gutil.log(gutil.colors.green("message:") + "some");
});


// 压缩html文件;
var htmlmin = require("gulp-htmlmin");
gulp.task("watchhtml", function () {
    gulp.watch("src/**/*.html", function (event) {
        var paths = watchPath(event, "src/", "dist/")
        gutil.log(gutil.colors.green(event.type) + '' + paths.srcPath);
        gutil.log("Dist" + paths.distPath);

        var combined = combiner.obj([
				gulp.src(paths.srcPath),
				//sourcemaps.init(),
			    htmlmin(),
				//sourcemaps.write("./"),
				gulp.dest(paths.distDir)
        ])
        combined.on("error", handleError)
    });
});

gulp.task("htmlmin", function () {
    var options = {
        collapseWhitespace: true,
        removeComments: true,
        removeEmptyAttributes: true,
        removeScriptTypeAttributes: true,
        removeStyleLinkTypeAttributes: true,
        minifyJS: true,
        minifyCSS: true
    };
    gulp.src("src/view/*.html")
        .pipe(htmlmin(options))
        .pipe(gulp.dest("dist/view/"));
});


// 合并js文件为一个文件;

var concat = require("gulp-concat");
//gulp.task("concatjs", function () {
//    //gulp.src(['1.js',['2.js']])  按照顺序进行合并的写法;
//    gulp.src("src/importJs/**/*.js")
//        .pipe(concat("main.js"))
//        .pipe(uglify())
//        .pipe(gulp.dest("dist/importJs"));
//})


// 一. 压缩js文件;
gulp.task("watchjs", function () {
    gulp.watch("src/js/**/*.js", function (event) {
        var paths = watchPath(event, "src/", "dist/")
        gutil.log(gutil.colors.green(event.type) + '' + paths.srcPath);
        gutil.log("Dist" + paths.distPath);

        var combined = combiner.obj([
				gulp.src(paths.srcPath),
				sourcemaps.init(),
				uglify(),
				sourcemaps.write("./"),
				gulp.dest(paths.distDir)
        ])
        combined.on("error", handleError)
    });
});
// 一次编辑所有js文件;
gulp.task("uglifyjs", function () {
    var combined = combiner.obj([
			gulp.src("src/js/**/*.js"),
			sourcemaps.init(),
			uglify(),
			sourcemaps.write("./"),
			gulp.dest("dist/js/")
    ])
    combined.on("error", handleError);
})

// 二. 配置css任务;
var minifycss = require("gulp-minify-css");
// 添加浏览器前缀到 css 规则里；
var autoprefixer = require("gulp-autoprefixer");

gulp.task("watchcss", function () {
    gulp.watch("src/css/**/*.css", function (event) {
        var paths = watchPath(event, "src/", "dist/");
        gutil.log(gutil.colors.green(event.type) + '' + paths.srcPath);
        gutil.log("Dist" + paths.distPath);

        gulp.src(paths.srcPath)
			 .pipe(sourcemaps.init())
			.pipe(autoprefixer({ browsers: 'Android 4.3' }))
			.pipe(minifycss())
			 .pipe(sourcemaps.write("./"))
			.pipe(gulp.dest(paths.distDir))
    })
});
// 一次编辑所有 css 文件
gulp.task("minifycss", function () {
    gulp.src("src/css/**/*.css")
	 .pipe(sourcemaps.init())
	.pipe(autoprefixer({ browsers: 'Android 4.3' }))
	.pipe(minifycss())
	 .pipe(sourcemaps.write("./"))
	.pipe(gulp.dest("dist/css/"))
});


//三. 配置less文件;
var less = require("gulp-less");
gulp.task("watchless", function () {
    gulp.watch("./src/less/**/*.less", function (event) {
        var paths = watchPath(event, "src/less/", "dist/css/");
        gutil.log(gutil.colors.green(event.type) + '' + paths.srcPath);
        gutil.log("Dist" + paths.distPath);

        var combined = combiner.obj([
				gulp.src(paths.srcPath),
				 sourcemaps.init(),
				autoprefixer({ browsers: 'Android 4.3' }),
				less(),
				minifycss(),
				 sourcemaps.write("./"),
				gulp.dest(paths.distDir)
        ])
        combined.on("error", handleError);
    });
});

// 一次性编辑所有less文件；
gulp.task("lesscss", function () {
    var combined = combiner.obj([
			gulp.src("src/less/**/*.less"),
			 sourcemaps.init(),
			autoprefixer({ browsers: 'Android 4.3' }),
			less(),
			minifycss(),
			 sourcemaps.write("./"),
			gulp.dest("dist/css/")
    ])
    combined.on("error", handleError)
});


// 四. 配置image 任务；
var imagemin = require("gulp-imagemin");
gulp.task("watchimage", function () {
    gulp.watch("src/images/**/*", function (event) {
        var paths = watchPath(event, "src/", "dist/");

        gutil.log(gutil.colors.green(event.type) + " " + paths.srcPath);
        gutil.log("Dist" + paths.distPath);
        gulp.src(paths.srcPath)
			.pipe(imagemin({ progressive: true }))
			.pipe(gulp.dest(paths.distDir))
    })
})

// 添加image任务
gulp.task("image", function () {
    gulp.src("src/images/**/*")
		.pipe(imagemin({ progressive: true }))
		.pipe(gulp.dest("dist/images"))
});
gulp.task("common", function () {
    gulp.src("src/common/**")
        .pipe(gulp.dest("dist/common"))
})
// 默认任务执行;
gulp.task("default", ["watchjs", "watchcss", "watchless", "watchimage", "watchhtml"]);
gulp.task("dist", ["uglifyjs", "minifycss", "lesscss",  "htmlmin","common"]);