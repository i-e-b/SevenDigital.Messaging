Dir["#{File.expand_path(File.dirname(__FILE__))}/lib/*.rb"].each {|file| require file }

path_to_nunit = "/usr/local/bin/nunit-console4"
$dot_net_path ||= "xbuild"

namespace :build do	
	
	desc 'Run the full build'
	task :full_build, [:solution_directory, :solution_file, :package_type, :database_build, :environment, :compilation_configuration, :compilation_verbosity] => [:clear, :test]
 
	desc 'Run build with no tests'
	task :no_test_build, [:solution_directory, :solution_file, :package_type, :database_build, :environment, :compilation_configuration, :compilation_verbosity] =>[:clear, :patch_assemblyinfo ,:compile, :recreate_database]

	
	desc 'Compile the solution'
	task :compile, :solution_directory, :solution_file, :environment, :compilation_configuration do |task, args|
		verbosity = "#{args.compilation_verbosity}" != "" ? "#{args.compilation_verbosity}" : "q"
		swap_configs("#{args.solution_directory}", "#{args.environment}")
		verbose(false) do
			puts "--------------[ Building #{args.solution_file} ]----------------"
			sh "#{$dot_net_path} #{args.solution_directory}/#{args.solution_file}"
		end
	end	
  
	desc 'Recreate the database'
	task :recreate_database, :server do |task, args|
		verbose(false) do
			server = "#{args.server}" != "" ? "#{args.server}" : "."
			temp_log = "temp_log.txt"   
			report = File.open("#{REPORT_DIRECTORY}/sql-report.txt", 'a')
			sorted_files = getSqlFiles(args, DATABASE_SCRIPTS_DIRECTORY)
			sorted_files.each do |script|
				if ((script.include? ".env.") && !(script.include? ".#{args.environment}.")) then
					puts "Skipping #{script} "
				else
					puts "Running #{script}"
					sh "mono ./build/sqlfk.exe -i \"#{script}\" -S \"devdb.sql.dev.svc.7d,59281\" -U testmanager -P XnW5rjsfraZe"
				end
			end
			report.close
			File.delete "#{temp_log}" if File.exists? "#{temp_log}"		
		end
  	end
 
	def getSqlFiles (args, database_scripts_directory)
		scriptsPath = "#{args.solution_directory}/#{database_scripts_directory}"
		if(File.directory?("#{scriptsPath}/SqlServer"))
			scriptsPath = "#{scriptsPath}/SqlServer"
		end
		Dir.glob(File.join("#{scriptsPath}/**", "*.sql")).sort_by {|f| File.basename f}
	end
 
	desc 'Run nunit unit tests'
  	task :test, [:solution_directory, :solution_file, :compilation_configuration] do |task, args|
		Dir.glob("#{args.solution_directory}/**/bin/**/*").each{|file| puts file}
    	specs = ["Specs.dll", "*.Specs.dll", "*.Specs.*.dll", "*.Tests.dll", "*.Tests.*.dll"].inject([]) { |files, pattern| 
        	Dir.glob(File.join("#{args.solution_directory}/**/bin/**/*", pattern)).each{|file|
				files += [file.concat(" ")]
			}
			puts "found NUnit assemblies: #{files}"
        	files
      	}
		if specs.length > 0
			specs.each_with_index{ |spec, i|
				sh "#{path_to_nunit} #{spec} -nologo -xml=#{args.solution_directory}/#{REPORT_DIRECTORY}/test-report-#{i}.xml; echo ''"
				puts "##teamcity[importData type='nunit' path='#{args.solution_directory}/#{REPORT_DIRECTORY}/test-report-#{i}.xml']"
			}
		else
			warn "No test assemblies found.  I find your lack of tests disturbing..."
			raise "No test assemblies found.  I find your lack of tests disturbing..."
		end 
	end

end


