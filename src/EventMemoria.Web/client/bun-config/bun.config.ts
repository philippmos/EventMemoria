import { basename } from 'path';

const mode = process.env.NODE_ENV || 'development';
export const isProduction = mode === 'production';

export const copyFiles = async (files: string[], outputDir: string) => {
    files.forEach(async (file) => {
        try {
            const fileContent = Bun.file(file);
            await Bun.write(`${outputDir}/${basename(file)}`, fileContent);
        } catch {
            console.error(`Error copying file ${file}`);
        }
    });
}
