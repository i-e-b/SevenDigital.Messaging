

namespace :build do	
	def log message
		sh "echo #{message}"
	end 
	
	def warn message
		log "Warning: #{message}"
	end
	
	def swap_configs(solution_directory, environment)
		Dir.glob(File.join(solution_directory, "**", "*.#{environment}")).each{|config_file|
			config_directory = File.dirname(config_file)
			master_config_file = Dir.glob(File.join(config_directory, File.basename(config_file, ".#{environment}"))).first
			if master_config_file != nil
				if FileUtils.compare_file(config_file, master_config_file)
				
				else
					FileUtils.cp(config_file, master_config_file)
				end
			end
		}
	end

	def get_build_number
		 
		tc_build_number = ENV['BUILD_NUMBER']
		if tc_build_number.nil?
			tc_build_number = 1
		end
		tc_build_number
	end

	desc 'Patch AssemblyInfo files with the version nummber before compile'
	task :patch_assemblyinfo do
		#puts "Finding AssemblyInfo files"
		assembly_info_files = AssemblyInfoFinder.new.find
		for file in assembly_info_files
			#puts "Updating #{file}"
			builder = AssemblyInfoBuilder.new(get_build_number, file)		
			builder.patch
		end
	end
end