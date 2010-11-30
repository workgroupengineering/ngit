/*
This code is derived from jgit (http://eclipse.org/jgit).
Copyright owners are documented in jgit's IP log.

This program and the accompanying materials are made available
under the terms of the Eclipse Distribution License v1.0 which
accompanies this distribution, is reproduced below, and is
available at http://www.eclipse.org/org/documents/edl-v10.php

All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided
  with the distribution.

- Neither the name of the Eclipse Foundation, Inc. nor the
  names of its contributors may be used to endorse or promote
  products derived from this software without specific prior
  written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using NGit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace NGit.Junit
{
	public class MockSystemReader : SystemReader
	{
		internal readonly IDictionary<string, string> values = new Dictionary<string, string
			>();

		internal FileBasedConfig userGitConfig;

		public MockSystemReader()
		{
			Init(Constants.OS_USER_NAME_KEY);
			Init(Constants.GIT_AUTHOR_NAME_KEY);
			Init(Constants.GIT_AUTHOR_EMAIL_KEY);
			Init(Constants.GIT_COMMITTER_NAME_KEY);
			Init(Constants.GIT_COMMITTER_EMAIL_KEY);
			userGitConfig = new _FileBasedConfig_70(null, null);
		}

		private sealed class _FileBasedConfig_70 : FileBasedConfig
		{
			public _FileBasedConfig_70(FilePath baseArg1, FS baseArg2) : base(baseArg1, baseArg2
				)
			{
			}

			/// <exception cref="System.IO.IOException"></exception>
			/// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
			public override void Load()
			{
			}

			// Do nothing
			public override bool IsOutdated()
			{
				return false;
			}
		}

		private void Init(string n)
		{
			SetProperty(n, n);
		}

		public virtual void ClearProperties()
		{
			values.Clear();
		}

		public virtual void SetProperty(string key, string value)
		{
			values.Put(key, value);
		}

		public override string Getenv(string variable)
		{
			return values.Get(variable);
		}

		public override string GetProperty(string key)
		{
			return values.Get(key);
		}

		public override FileBasedConfig OpenUserConfig(FS fs)
		{
			return userGitConfig;
		}

		public override string GetHostname()
		{
			return "fake.host.example.com";
		}

		public override long GetCurrentTime()
		{
			return 1250379778668L;
		}

		// Sat Aug 15 20:12:58 GMT-03:30 2009
		public override int GetTimezone(long when)
		{
			return Sharpen.Extensions.GetTimeZone("GMT-03:30").GetOffset(when) / (60 * 1000);
		}
	}
}