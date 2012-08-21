
Dir["#{File.expand_path(File.dirname(__FILE__))}/lib/*.rb"].each {|file| require file }


tools_directory = "Build/Tools"
database_scripts_directory = "DatabaseScripts"
current_database_scripts_directory = "#{database_scripts_directory}/Current"

output_directory = "Build/Output"
package_directory = "#{output_directory}"
package_database_scripts_directory = "#{package_directory}/DatabaseScripts"
report_directory = "#{output_directory}/Reports"
$dot_net_path ||= "#{ENV["SystemRoot"]}/Microsoft.NET/Framework/v4.0.30319/"


namespace :build do	
	
	desc 'Run the full build'
	task :full_build, [:solution_directory, :solution_file, :package_type, :database_build, :environment, :compilation_configuration, :compilation_verbosity] => [:clear, :test]
 
	desc 'Run build with no tests'
	task :no_test_build, [:solution_directory, :solution_file, :package_type, :database_build, :environment, :compilation_configuration, :compilation_verbosity] =>[:clear, :compile, :recreate_database]

	desc 'Clear the output directory'
	task :clear do
		if (File.exists?(report_directory) and File.directory?(report_directory))
			FileUtils.rm_r report_directory
		end
		verbose (false) do
				mkdir_p [report_directory]		 
		end
	end
	
	desc 'Compile the solution'
	task :compile, :solution_directory, :solution_file, :environment, :compilation_configuration do |task, args|
		verbosity = "#{args.compilation_verbosity}" != "" ? "#{args.compilation_verbosity}" : "q"
		swap_configs("#{args.solution_directory}", "#{args.environment}")
		verbose(false) do
			puts "--------------[ Building #{args.solution_file} ]----------------"
			sh "#{$dot_net_path}msbuild.exe /v:#{verbosity} /p:Configuration=#{args.compilation_configuration} #{args.solution_directory}/#{args.solution_file}"
		end
	end	
  
	desc 'Recreate the database'
	task :recreate_database, :server do |task, args|
		verbose(false) do
			server = "#{args.server}" != "" ? "#{args.server}" : ".\\SQLEXPRESS"
			temp_log = "temp_log.txt"   
			report = File.open("#{report_directory}/sql-report.txt", 'a')
			sorted_files = Dir.glob(File.join("#{args.solution_directory}/#{database_scripts_directory}/**", "*.sql")).sort_by {|f| File.basename f}
			sorted_files.each do |script|
				if ((script.include? ".env.") && !(script.include? ".#{args.environment}.")) then
					puts "Skipping #{script} "
				else
					puts "Running #{script}"
					sh "sqlcmd.exe -i \"#{script}\" -S #{server} -E -o #{temp_log}  "
					log = File.open("#{temp_log}", 'r')
					report << "#{log.readlines}\r\n"
					log.close
				end
			end
			report.close
			File.delete "#{temp_log}" if File.exists? "#{temp_log}"		
		end
  	end
 
	desc 'Run nunit unit tests'
  	task :test, [:solution_directory, :solution_file, :compilation_configuration] do |task, args|
    	specs = ["Specs.dll", "*.Specs.dll", "*.Specs.*.dll", "*.Tests.dll", "*.Tests.*.dll"].inject([]) { |files, pattern| 
        	Dir.glob(File.join("#{args.solution_directory}/**/bin/**/#{args.compilation_configuration}", pattern)).each{|file|
				files += [file.concat(" ")]
			}
			puts "found NUnit assemblies: #{files}"
        	files
      	}
		if specs.length > 0
			specs.each_with_index{ |spec, i|
				sh "#{tools_directory}/NUnit/nunit-console.exe #{spec} /nologo /xml=#{report_directory}/test-report-#{i}.xml"
				puts "##teamcity[importData type='nunit' path='#{report_directory}/test-report-#{i}.xml']"
			}
		else
			warn "No test assemblies found.  I find your lack of tests disturbing..."
		end 
	end   
end
