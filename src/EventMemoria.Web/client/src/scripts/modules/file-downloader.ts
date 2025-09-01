class FileDownloader {

    /**
     * Downloads a file from a data URL
     * @param dataUrl - The data URL of the file to download
     * @param fileName - The name for the downloaded file
     */
    public static downloadFile(dataUrl: string, fileName: string): void {
        const link = document.createElement('a');
        link.href = dataUrl;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
}

export default FileDownloader;
