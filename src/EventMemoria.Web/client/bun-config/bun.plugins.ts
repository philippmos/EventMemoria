import { BunPlugin } from 'bun';
import * as sass from 'sass';

export const styleScss: BunPlugin = {
    name: 'Sass Loader',
    async setup(build: any) {
        build.onLoad({ filter: /\.scss$/ }, async ({ path }: { path: string }) => {
            const result = sass.compile(path, { style: "expanded" });
            const css = result.css.toString().trim();

            return {
                loader: "css",
                contents: css
            };
        });
    }
};