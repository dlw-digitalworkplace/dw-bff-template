import { fileURLToPath, URL } from "node:url";
import { defineConfig } from "vite";
import { env } from "process";
import plugin from "@vitejs/plugin-react";
import fs from "fs";
import path from "path";
import child_process from "child_process";

// Load DEV certificate
const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ""
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = "dlw.bff.template.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(baseFolder)) {
    fs.mkdirSync(baseFolder, { recursive: true });
}

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (0 !== child_process.spawnSync("dotnet", [
        "dev-certs",
        "https",
        "--export-path",
        certFilePath,
        "--format",
        "Pem",
        "--no-password",
    ], { stdio: "inherit", }).status) {
        throw new Error("Could not create certificate.");
    }
}

// https://vitejs.dev/config/
const target = "https://localhost:7057";
export default defineConfig({
    plugins: [plugin()],
    resolve: {
        alias: {
            "@": fileURLToPath(new URL("./src", import.meta.url))
        }
    },
    server: {
        proxy: {
            "^/api": {
                target,
                secure: false
            },
            "^/microsoftidentity": {
                target,
                secure: false
            },
            "^/signin-oidc": {
                target,
                secure: false
            }
        },
        port: 57943,
        https: {
            key: fs.readFileSync(keyFilePath),
            cert: fs.readFileSync(certFilePath),
        }
    }
})
