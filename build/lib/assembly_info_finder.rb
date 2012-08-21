class AssemblyInfoFinder 

	def find
		assemblyinfo_files = Dir.glob("#{File.dirname(__FILE__)}/../../**/AssemblyInfo.*")
		puts "found the following assemblyinfo files #{assemblyinfo_files.to_s}"
		if (assemblyinfo_files.length < 1)
			raise "No AssemblyInfo files found"
		end
		
		#puts "Found #{assemblyinfo_files.length} AssemblyInfo files"
		assemblyinfo_files
	end

end
