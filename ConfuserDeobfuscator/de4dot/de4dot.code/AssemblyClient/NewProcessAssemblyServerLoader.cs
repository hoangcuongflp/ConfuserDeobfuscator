﻿/*
    Copyright (C) 2011-2013 de4dot@gmail.com

    This file is part of de4dot.

    de4dot is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    de4dot is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with de4dot.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;

namespace de4dot.code.AssemblyClient {
	// Starts the server in a new process
	class NewProcessAssemblyServerLoader : IpcAssemblyServerLoader {
		Process process;

		public NewProcessAssemblyServerLoader() {
		}

		public NewProcessAssemblyServerLoader(ServerClrVersion version)
			: base(version) {
		}

		public override void loadServer(string filename) {
			if (process != null)
				throw new ApplicationException("Server is already loaded");

			var psi = new ProcessStartInfo {
				Arguments = string.Format("{0} {1}", Utils.shellEscape(ipcName), Utils.shellEscape(ipcUri)),
				CreateNoWindow = true,
				ErrorDialog = false,
				FileName = filename,
				LoadUserProfile = false,
				UseShellExecute = false,
				WorkingDirectory = Utils.getOurBaseDir(),
			};
			process = Process.Start(psi);
			if (process == null)
				throw new ApplicationException("Could not start process");
		}

		public override void Dispose() {
			if (process != null) {
				if (!process.WaitForExit(300)) {
					try {
						process.Kill();
					}
					catch (InvalidOperationException) {
						// Here if process has already exited.
					}
				}
				process = null;
			}
		}
	}
}
