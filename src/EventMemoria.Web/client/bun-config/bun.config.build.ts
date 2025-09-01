import { copyFiles, isProduction } from './bun.config';
import { styleScss } from './bun.plugins';

await Bun.build({
    entrypoints: ['./src/scripts/main.ts'],
    outdir: '../wwwroot/js',
    naming: 'index.[ext]',
    target: 'browser',
    minify: isProduction,
    env: 'inline'
});

await Bun.build({
    entrypoints: ['./src/styles/main.scss'],
    outdir: '../wwwroot/css',
    naming: '[name].css',
    minify: isProduction,
    loader: { '.scss': 'css' },
    plugins: [styleScss]
});

await copyFiles(['./src/assets/favicon.ico'], '../wwwroot');
