class AssemblyInfoBuilder
	attr_reader :attributes

	def initialize(build_number, template)
		@build_number = build_number
    	@template = template
	end
	
	def get_extension
	  @template[(@template.length-2), @template.length]
	end

	def patch()
    @content = ""

	
    File.open(@template, 'r') do |file|
      file.readlines.each { |line|
        @content << line.
          gsub(/.*assembly\s*:\s*AssemblyFileVersion.*/i, '').
          gsub(/.*assembly\s*:\s*AssemblyVersion.*/i, '')
      }
    end

    @content = @content.strip
    
	
	brackets = get_extension == "vb" ? {:opening => "<", :closing => ">"} : {:opening => "[", :closing => "]"}
	
	
    @content << "\n#{brackets[:opening]}assembly: AssemblyFileVersion(\"#{@build_number}\")#{brackets[:closing]}\n"
	
    @content << "#{brackets[:opening]}assembly: AssemblyVersion(\"#{@build_number}\")#{brackets[:closing]}\n"

		file = File.open(@template, 'w')
    file << @content
    file.close
		
	
	end
end