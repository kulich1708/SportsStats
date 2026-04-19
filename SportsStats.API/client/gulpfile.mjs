import gulp from 'gulp';
import gulpSass from 'gulp-sass';
import * as dartSass from 'sass';
import * as esbuild from 'esbuild';
import { fileURLToPath } from 'url';
import { dirname, join } from 'path';

const __dirname = dirname(fileURLToPath(import.meta.url));
const wwwroot = join(__dirname, '..', 'wwwroot');

const sassCompiler = gulpSass(dartSass);

function scss() {
  return gulp
    .src('src/styles/main.scss', { sourcemaps: true })
    .pipe(sassCompiler.sync().on('error', sassCompiler.logError))
    .pipe(gulp.dest(join(wwwroot, 'css'), { sourcemaps: '.' }));
}

async function js() {
  await esbuild.build({
    entryPoints: ['src/main.ts'],
    bundle: true,
    outfile: join(wwwroot, 'js', 'bundle.js'),
    format: 'iife',
    target: ['es2020'],
    minify: false,
    sourcemap: true,
  });
}

export const build = gulp.parallel(scss, js);

export function watch() {
  gulp.watch('src/**/*.scss', scss);
  gulp.watch('src/**/*.ts', js);
}

export default build;
